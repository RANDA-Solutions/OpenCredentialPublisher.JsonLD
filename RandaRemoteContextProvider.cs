//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using VDS.RDF.JsonLd;
//using VDS.RDF.JsonLd.Syntax;

//namespace OpenCredentialPublisher.JsonLD
//{
//    public class RandaRemoteContextProvider : IRemoteContextProvider
//    {
//        private readonly JsonLdProcessorOptions _options;
//        private readonly Dictionary<Uri, JsonLdRemoteContext> _remoteContextCache;

//        /// <summary>
//        /// Create a new provider instance configured using the specified options.
//        /// </summary>
//        /// <param name="options"></param>
//        public RandaRemoteContextProvider(JsonLdProcessorOptions options)
//        {
//            _options = options;
//            _remoteContextCache = new Dictionary<Uri, JsonLdRemoteContext>();
//        }

//        /// <inheritdoc />
//        public JsonLdRemoteContext GetRemoteContext(Uri reference)
//        {
//            if (_remoteContextCache.TryGetValue(reference, out JsonLdRemoteContext cachedContext)) return cachedContext;
//            try
//            {
//                RemoteDocument remoteDoc = LoadJson(reference,
//                    new JsonLdLoaderOptions
//                    { Profile = JsonLdVocabulary.Context, RequestProfile = JsonLdVocabulary.Context }, _options);
//                JToken jsonRepresentation = GetJsonRepresentation(remoteDoc);
//                if (!(jsonRepresentation is JObject remoteJsonObject))
//                {
//                    throw new JsonLdProcessorException(JsonLdErrorCode.InvalidRemoteContext,
//                        $"Remote document at {reference} could not be parsed as a JSON object.");
//                }

//                if (!remoteJsonObject.ContainsKey("@context"))
//                {
//                    throw new JsonLdProcessorException(JsonLdErrorCode.InvalidRemoteContext,
//                        $"Remote document at {reference} does not have an @context property");
//                }

//                var remoteContext = CreateInstance<JsonLdRemoteContext>(remoteDoc.DocumentUrl, remoteJsonObject["@context"]);
//                _remoteContextCache[reference] = remoteContext;
//                return remoteContext;

//            }
//            catch (JsonLdProcessorException)
//            {
//                throw;
//            }
//            catch (Exception ex)
//            {
//                throw new JsonLdProcessorException(JsonLdErrorCode.LoadingRemoteContextFailed,
//                    $"Failed to load remote context from {reference}.", ex);
//            }
//        }

//        private static RemoteDocument LoadJson(Uri remoteRef, JsonLdLoaderOptions loaderOptions,
//            JsonLdProcessorOptions options)
//        {
//            return options.DocumentLoader != null
//                ? options.DocumentLoader(remoteRef, loaderOptions)
//                : DefaultDocumentLoader.LoadJson(remoteRef, loaderOptions);
//        }

//        private static JToken GetJsonRepresentation(RemoteDocument remoteDoc)
//        {
//            switch (remoteDoc.Document)
//            {
//                case JToken representation:
//                    return representation;
//                case string docStr:
//                    {
//                        try
//                        {

//                            return JToken.Parse(docStr);
//                        }
//                        catch (Exception ex)
//                        {
//                            throw new JsonLdProcessorException(JsonLdErrorCode.InvalidRemoteContext,
//                                "Could not parse remote content as a JSON document. ", ex);
//                        }
//                    }
//                default:
//                    throw new JsonLdProcessorException(JsonLdErrorCode.InvalidRemoteContext,
//                        "Could not parse remote content as a JSON document.");
//            }
//        }

//        public static T CreateInstance<T>(params object[] args)
//        {
//            var type = typeof(T);
//            var instance = type.Assembly.CreateInstance(
//                type.FullName, false,
//                BindingFlags.Instance | BindingFlags.NonPublic,
//                null, args, null, null);
//            return (T)instance;
//        }
//    }
//}
