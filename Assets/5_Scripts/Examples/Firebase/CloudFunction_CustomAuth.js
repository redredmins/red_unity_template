/*
 * ============================================================================
 * Cloud Function: 카카오/네이버 Custom Token 발급
 * ============================================================================
 *
 * 이 파일은 Firebase Cloud Functions에 배포할 Node.js 코드 샘플입니다.
 * Unity에서는 사용되지 않으며, 참고용 문서입니다.
 *
 * ============================================================================
 * 배포 방법
 * ============================================================================
 *
 * 1. Firebase CLI 설치:
 *    npm install -g firebase-tools
 *
 * 2. 프로젝트 초기화:
 *    firebase init functions
 *
 * 3. 이 코드를 functions/index.js에 복사
 *
 * 4. 종속성 설치:
 *    cd functions && npm install node-fetch@2
 *
 * 5. 배포:
 *    firebase deploy --only functions
 *
 * ============================================================================
 * 코드
 * ============================================================================
 */

const functions = require("firebase-functions");
const admin = require("firebase-admin");
const fetch = require("node-fetch");

admin.initializeApp();

// ============================================================================
// 카카오 로그인 Custom Token 발급
// ============================================================================
// 클라이언트에서 카카오 SDK로 획득한 액세스 토큰을 검증하고
// Firebase Custom Token을 발급합니다.

exports.createKakaoCustomToken = functions.https.onRequest(async (req, res) => {
  if (req.method !== "POST") {
    res.status(405).send({ error: "Method not allowed" });
    return;
  }

  const { token } = req.body;

  if (!token) {
    res.status(400).send({ error: "Missing access token" });
    return;
  }

  try {
    // 1. 카카오 API로 액세스 토큰 검증
    const kakaoResponse = await fetch("https://kapi.kakao.com/v2/user/me", {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!kakaoResponse.ok) {
      res.status(401).send({ error: "Invalid Kakao token" });
      return;
    }

    const kakaoUser = await kakaoResponse.json();
    const kakaoUid = `kakao:${kakaoUser.id}`;

    // 2. Firebase Custom Token 생성
    const customToken = await admin.auth().createCustomToken(kakaoUid, {
      provider: "kakao",
      kakaoId: kakaoUser.id.toString(),
    });

    // 3. (선택) Firebase Auth에 유저 레코드 생성/업데이트
    try {
      await admin.auth().getUser(kakaoUid);
    } catch (e) {
      // 유저가 없으면 생성
      if (e.code === "auth/user-not-found") {
        await admin.auth().createUser({
          uid: kakaoUid,
          displayName:
            kakaoUser.kakao_account?.profile?.nickname || "Kakao User",
        });
      }
    }

    res.send({ customToken: customToken });
  } catch (error) {
    console.error("Kakao auth error:", error);
    res.status(500).send({ error: "Internal server error" });
  }
});

// ============================================================================
// 네이버 로그인 Custom Token 발급
// ============================================================================

exports.createNaverCustomToken = functions.https.onRequest(async (req, res) => {
  if (req.method !== "POST") {
    res.status(405).send({ error: "Method not allowed" });
    return;
  }

  const { token } = req.body;

  if (!token) {
    res.status(400).send({ error: "Missing access token" });
    return;
  }

  try {
    // 1. 네이버 API로 액세스 토큰 검증
    const naverResponse = await fetch("https://openapi.naver.com/v1/nid/me", {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!naverResponse.ok) {
      res.status(401).send({ error: "Invalid Naver token" });
      return;
    }

    const naverData = await naverResponse.json();

    if (naverData.resultcode !== "00") {
      res.status(401).send({ error: "Naver token verification failed" });
      return;
    }

    const naverUser = naverData.response;
    const naverUid = `naver:${naverUser.id}`;

    // 2. Firebase Custom Token 생성
    const customToken = await admin.auth().createCustomToken(naverUid, {
      provider: "naver",
      naverId: naverUser.id,
    });

    // 3. (선택) Firebase Auth에 유저 레코드 생성/업데이트
    try {
      await admin.auth().getUser(naverUid);
    } catch (e) {
      if (e.code === "auth/user-not-found") {
        await admin.auth().createUser({
          uid: naverUid,
          displayName: naverUser.name || naverUser.nickname || "Naver User",
        });
      }
    }

    res.send({ customToken: customToken });
  } catch (error) {
    console.error("Naver auth error:", error);
    res.status(500).send({ error: "Internal server error" });
  }
});

// ============================================================================
// 범용 Custom Token 발급 (여러 제공자를 하나의 엔드포인트로)
// ============================================================================
// 클라이언트에서 { token: "...", provider: "Kakao" } 형태로 요청합니다.

exports.createCustomToken = functions.https.onRequest(async (req, res) => {
  if (req.method !== "POST") {
    res.status(405).send({ error: "Method not allowed" });
    return;
  }

  const { token, provider } = req.body;

  if (!token || !provider) {
    res.status(400).send({ error: "Missing token or provider" });
    return;
  }

  const providerConfig = {
    Kakao: {
      url: "https://kapi.kakao.com/v2/user/me",
      getUid: (data) => `kakao:${data.id}`,
      getName: (data) => data.kakao_account?.profile?.nickname || "Kakao User",
    },
    Naver: {
      url: "https://openapi.naver.com/v1/nid/me",
      getUid: (data) => `naver:${data.response.id}`,
      getName: (data) =>
        data.response.name || data.response.nickname || "Naver User",
    },
    // 다른 제공자를 여기에 추가하세요
  };

  const config = providerConfig[provider];
  if (!config) {
    res.status(400).send({ error: `Unsupported provider: ${provider}` });
    return;
  }

  try {
    const response = await fetch(config.url, {
      headers: { Authorization: `Bearer ${token}` },
    });

    if (!response.ok) {
      res.status(401).send({ error: `Invalid ${provider} token` });
      return;
    }

    const userData = await response.json();
    const uid = config.getUid(userData);

    const customToken = await admin.auth().createCustomToken(uid, {
      provider: provider.toLowerCase(),
    });

    // 유저 레코드 생성 (첫 로그인 시)
    try {
      await admin.auth().getUser(uid);
    } catch (e) {
      if (e.code === "auth/user-not-found") {
        await admin.auth().createUser({
          uid: uid,
          displayName: config.getName(userData),
        });
      }
    }

    res.send({ customToken: customToken });
  } catch (error) {
    console.error(`${provider} auth error:`, error);
    res.status(500).send({ error: "Internal server error" });
  }
});
