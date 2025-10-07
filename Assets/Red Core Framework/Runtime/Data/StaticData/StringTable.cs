using UnityEngine;
using System.Collections.Generic;

namespace RedMinS
{
    public class StringTable
    {
        Dictionary<int, string> stringTable = null;


        public StringTable(TextAsset tableText)
        {
            stringTable = new Dictionary<int, string>();
            MakeStringTable(tableText);
        }

        public StringTable(TextAsset[] tableTexts)
        {
            stringTable = new Dictionary<int, string>();
            foreach(var tableText in tableTexts)
            {
                MakeStringTable(tableText);
            }
        }

        public void MakeStringTable(TextAsset tableText)
        {
            //stringTable.Clear();

            Dictionary<string, object> table =
                (Dictionary<string, object>)MiniJSON.Json.Deserialize(tableText.text);

            //Debug.Log(tableText.text);

            foreach (KeyValuePair<string, object> pair in table)
            {
                int uid = int.Parse(pair.Key);
                string script = pair.Value.ToString();

                stringTable.Add(uid, script);
            }
        }

        //
        public string GetString(int uid)
        {
            if (stringTable.ContainsKey(uid))
            {
                return stringTable[uid];
            }
            else
                return "-";
        }

    }
}