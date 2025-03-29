using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Localization;

namespace DialogueSystem {
    public class LocalizedStringId {
        [JsonProperty]
        public string collectionId;
        [JsonProperty]
        public string key;

        public LocalizedString localizedString() {
            return new LocalizedString(collectionId, key);
        } 

        public static LocalizedStringId construct(LocalizedString localizedString) {
            return new LocalizedStringId(localizedString.TableReference.TableCollectionName,
                localizedString.TableEntryReference.Key);
        }
        
        public LocalizedStringId(string s, string s1) {
            collectionId = s;
            key = s1;
        }
        
    }
}