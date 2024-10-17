using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using JsonLd.Normalization;

namespace OpenCredentialPublisher.JsonLD
{
    public class JsonLinkedDataSignature
    {
        public byte[] CreateVerifyData(JObject document, JToken proof)
        {
            return CreateVerifyDataAsync(document, proof).Result;
        }

        public async Task<byte[]> CreateVerifyDataAsync(JObject document, JToken proof)
        {
            var documentHash = await CanonizeDocumentAsync(document);
            var proofOptionsHash = await CanonizeProofAsync(document, proof);
            var array = new Byte[proofOptionsHash.Length + documentHash.Length];
            proofOptionsHash.CopyTo(array, 0);
            documentHash.CopyTo(array, proofOptionsHash.Length);
            return array;
        }

        public async Task<byte[]> CanonizeDocumentAsync(JObject document)
        {
            var normalizedJson = await JsonLdHandler.Normalize(document.ToString(), new ExpandOptions
            {
                Base = "c14n"
            });

            using (var sha256 = SHA256.Create())
            {
                var textBytes = Encoding.UTF8.GetBytes(normalizedJson);
                var hashedBytes = sha256.ComputeHash(textBytes);
                return hashedBytes;
            }
        }

        public async Task<byte[]> CanonizeProofAsync(JObject document, JToken proof)
        {
            var serializedProof = JsonSerializer.Serialize(proof);
            var proofDocument = JObject.Parse("{}");
#if NETSTANDARD2_1
            proofDocument.TryAdd("@context", document["@context"]);
#else
            proofDocument.Add("@context", document["@context"]);
#endif

            proofDocument.Merge(proof);

            proofDocument.Remove("jws");
            proofDocument.Remove("signatureValue");
            proofDocument.Remove("proofValue");
            proofDocument.Remove("signature");

            return await CanonizeDocumentAsync(proofDocument);
        }
    }
}
