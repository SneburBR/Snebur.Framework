using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Serializacao;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Snebur.Utilidade
{
    public static class JsonUtil
    {
        //IsoDateFormat varios problemas
        //MicrosoftDateFormat 

        private static readonly JsonSerializerSettings ConfiguracoesDeserializar = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
        };

        private static readonly JsonSerializerSettings ConfiguracoesSerializarJavascript = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new JsonPropertiesResolver(),
        };

        private static readonly JsonSerializerSettings ConfiguracoesSerializarDotNet = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new JsonPropertiesResolver()
        };

        public static T DeserializaArquivor<T>(string caminhoArquivo, Encoding encoding, bool isJavascript = true)
        {
            using (var fs = StreamUtil.OpenRead(caminhoArquivo))
            using (var sr = new StreamReader(fs, encoding))
            {
                var json = sr.ReadToEnd();
                return JsonUtil.Deserializar<T>(json, isJavascript);
            }
        }

        public static T Deserializar<T>(string json, bool isJavascript = true)
        {
            if (String.IsNullOrEmpty(json))
            {
                return default(T);
            }
            return (T)Deserializar(json, typeof(T), isJavascript);
        }

        public static object Deserializar(string json, Type tipo)
        {
            return JsonUtil.Deserializar(json, tipo, false);
        }

        public static object Deserializar(string json, Type tipo, bool isJavascript)
        {
            try
            {
                if (String.IsNullOrEmpty(json))
                {
                    return null;
                }
                var configuracaoSerializacao = isJavascript ? JsonUtil.ConfiguracoesSerializarJavascript :
                                                              JsonUtil.ConfiguracoesSerializarDotNet;

                var objeto = JsonConvert.DeserializeObject(json, tipo, configuracaoSerializacao);
                if (!isJavascript)
                {
                    using (var normalizar = new NormalizarDeserializacao(json, objeto))
                    {
                        normalizar.Normalizar();
                    }
                }
                return objeto;
            }
            catch (Exception ex)
            {
                if (ex is ErroSerializacao)
                {
                    throw;
                }
                throw new ErroSerializacao(json, ex);
            }
        }
        //public static string Serializar(object objeto)
        //{
        //    return JsonUtil.Serializar(objeto, false);
        //}

        public static string Serializar(object objeto, bool isJavascript)
        {
            return JsonUtil.Serializar(objeto, isJavascript, Debugger.IsAttached);
        }

        public static string Serializar(object objeto, bool isJavascript, bool isIdentar)
        {
            if (objeto == null)
            {
                return "null";
            }
            try
            {
                var formatacaoJson = (isIdentar) ? Formatting.Indented :
                                                   Formatting.None;

                var configuracaoSerializacao = isJavascript ? JsonUtil.ConfiguracoesSerializarJavascript :
                                                              JsonUtil.ConfiguracoesSerializarDotNet;

                using (var preparar = new PrapararSerializacao(objeto))
                {
                    try
                    {
                        preparar.Preparar();
                        return JsonConvert.SerializeObject(objeto, formatacaoJson, configuracaoSerializacao);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        preparar.Normalizar();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ErroSerializacao(objeto, ex);
            }
        }

        public static string SerializarJsonCamelCase(object obj)
        {
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(obj, serializerSettings);
        }

        public static void SalvarSerializacao(object objecto,
                                             bool isJavascript,
                                             string caminhoDestino,
                                             bool isIdentar = true)
        {
            var json = JsonUtil.Serializar(objecto, isJavascript, isIdentar);
            ArquivoUtil.DeletarArquivo(caminhoDestino);
            ArquivoUtil.SalvarArquivoTexto(caminhoDestino, json);
        }
        public static List<MemberInfo> RetornarPropriedadesSerializavel(Type objectType, bool isCamposBaseDominio = false)
        {
            var memberInfos = objectType.GetProperties().Where(x => x.GetMethod != null &&
                                                                    x.SetMethod != null &&
                                                                    x.GetMethod.IsPublic &&
                                                                    !Attribute.IsDefined(x, typeof(IgnorarPropriedadeTSAttribute)) &&
                                                                    !Attribute.IsDefined(x, typeof(JsonIgnoreAttribute)) &&
#if NET50
                                                                    !Attribute.IsDefined(x, typeof(System.Text.Json.Serialization.JsonIgnoreAttribute)) &&
#endif
                                                                    !Attribute.IsDefined(x, typeof(XmlIgnoreAttribute)))
                                                        .ToList<MemberInfo>();

            if (isCamposBaseDominio && objectType.IsSubclassOf(typeof(BaseDominio)))
            {
                var flags = BindingFlags.NonPublic | BindingFlags.Instance;
                var tipoBaseDominio = typeof(BaseDominio);
                memberInfos.Add(tipoBaseDominio.GetField(nameof(IBaseDominioReferencia.__IdentificadorUnico), flags));
                memberInfos.Add(tipoBaseDominio.GetField(nameof(IBaseDominioReferencia.__IdentificadorReferencia), flags));
                memberInfos.Add(tipoBaseDominio.GetField(nameof(IBaseDominioReferencia.__IsBaseDominioReferencia), flags));
            }
            return memberInfos;
        }
    }

    internal class JsonPropertiesResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            return JsonUtil.RetornarPropriedadesSerializavel(objectType, true);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var jsonPropriedade = base.CreateProperty(member, memberSerialization);
            if (member is PropertyInfo propriedade)
            {
                if (propriedade.PropertyType != typeof(string))
                {
                    if (propriedade.PropertyType.GetInterface(nameof(ICollection)) != null)
                    {
                        jsonPropriedade.ShouldSerialize = (instancia) =>
                        {
                            try
                            {
                                var valorPropriedade = propriedade.GetValue(instancia);
                                if (valorPropriedade is ICollection colecao)
                                {
                                    return colecao.Count > 0;
                                }
                            }
                            catch
                            {
                            }
                            return false;
                        };
                        //jsonPropriedade.ShouldSerialize =
                        //    instance => (instance?.GetType().GetProperty(jsonPropriedade.PropertyName).GetValue(instance) as IEnumerable<object>)?.Count() > 0;
                    }
                }
            }
            return jsonPropriedade;
        }
    }
}