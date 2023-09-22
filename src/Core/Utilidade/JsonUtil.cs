using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Snebur.Dominio;
using Snebur.Dominio.Atributos;
using Snebur.Serializacao;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Snebur.Utilidade
{
    public static class JsonUtil
    {

        private static IReferenceResolver _referenceResolver;

        public static IReferenceResolver ReferenceResolver => LazyUtil.RetornarValorLazyComBloqueio(ref _referenceResolver, () => new ResolverReferencia());

        private static readonly JsonSerializerSettings ConfiguracoesJavascript = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new JsonPropertiesResolver(),
        };

        private static readonly JsonSerializerSettings ConfiguracoesDotNet = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new JsonPropertiesResolver(),
            ReferenceResolverProvider = () =>
            {
                return JsonUtil.ReferenceResolver;
            }

        };

        public static T DeserializaArquivor<T>(string caminhoArquivo, EnumTipoSerializacao tipoSerializacao)
        {
            return DeserializaArquivor<T>(caminhoArquivo, Encoding.UTF8, tipoSerializacao);
        }

        public static T DeserializaArquivor<T>(string caminhoArquivo,
                                              Encoding encoding,
                                              EnumTipoSerializacao tipoSerializacao)
        {
            using (var fs = StreamUtil.OpenRead(caminhoArquivo))
            using (var sr = new StreamReader(fs, encoding))
            {
                var json = sr.ReadToEnd();
                return JsonUtil.Deserializar<T>(json, tipoSerializacao);
            }
        }

        public static T TryDeserializar<T>(string json, EnumTipoSerializacao tipoSerializacao)
        {
            try
            {
                return JsonUtil.Deserializar<T>(json, tipoSerializacao);
            }
            catch
            {
                return default;
            }
        }

        public static T Deserializar<T>(string json, EnumTipoSerializacao tipoSerializacao)
        {
            return Deserializar<T>(json, tipoSerializacao, CultureInfo.InvariantCulture);
        }
        public static T Deserializar<T>(string json,
                                        EnumTipoSerializacao tipoSerializacao,
                                        CultureInfo culture)
        {
            if (String.IsNullOrEmpty(json))
            {
                return default(T);
            }
            return (T)Deserializar(json, typeof(T), tipoSerializacao, culture);
        }

        public static object Deserializar(string json,
                                          Type tipo,
                                          EnumTipoSerializacao tipoSerializacao)
        {
            return Deserializar(json,
                                tipo,
                                tipoSerializacao,
                                CultureInfo.InvariantCulture);
        }
        public static object Deserializar(string json,
                                      Type tipo,
                                      EnumTipoSerializacao tipoSerializacao,
                                      CultureInfo culture)
        {
            try
            {
                if (String.IsNullOrEmpty(json))
                {
                    return null;
                }

                var configuracaoSerializacao = tipoSerializacao == EnumTipoSerializacao.Javascript ? JsonUtil.ConfiguracoesJavascript :
                                                                                                     JsonUtil.ConfiguracoesDotNet;

                configuracaoSerializacao.Culture = culture ?? CultureInfo.InvariantCulture;

                var objeto = JsonConvert.DeserializeObject(json, tipo, configuracaoSerializacao);
                if (tipoSerializacao == EnumTipoSerializacao.DotNet)
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

        public static void Serializar(object objeto,
                                      EnumTipoSerializacao tipoSerializacao,
                                      string caminhoArquivo)
        {
            SalvarSerializacao(objeto, tipoSerializacao, caminhoArquivo);
        }
        public static string Serializar(object objeto,
                                        EnumTipoSerializacao tipoSerializacao)
        {
            return JsonUtil.Serializar(objeto,
                                       tipoSerializacao,
                                       cultureInfo: CultureInfo.InvariantCulture,
                                       isIdentar: DebugUtil.IsAttached,
                                       isPrepararSerializacao: true);
        }

        public static string Serializar(object objeto,
                                        EnumTipoSerializacao tipoSerializacao,
                                        CultureInfo cultureInfo,
                                        bool isIdentar,
                                        bool isPrepararSerializacao = true)
        {
            if (objeto == null)
            {
                return "null";
            }
            try
            {
                var formatacaoJson = (isIdentar) ? Formatting.Indented :
                                                   Formatting.None;

                var configuracaoSerializacao = tipoSerializacao == EnumTipoSerializacao.Javascript
                                                    ? JsonUtil.ConfiguracoesJavascript
                                                    : JsonUtil.ConfiguracoesDotNet;

                configuracaoSerializacao.Culture = cultureInfo ?? CultureInfo.InvariantCulture;


                if (!isPrepararSerializacao)
                {
                    return JsonConvert.SerializeObject(objeto,
                                                       formatacaoJson,
                                                       configuracaoSerializacao);
                }

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
                                             EnumTipoSerializacao tipoSerializacao,
                                             string caminhoDestino,
                                             bool isIdentar = true)
        {
            var json = JsonUtil.Serializar(objecto,
                                          tipoSerializacao,
                                          CultureInfo.InvariantCulture,
                                          isIdentar,
                                          false);

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
#if NET7_0
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

        public static string IndentarJson(string json,
                                          CultureInfo culture)
        {
            try
            {
                var objeto = JsonUtil.Deserializar<object>(json, EnumTipoSerializacao.Javascript, culture);
                return JsonUtil.Serializar(objeto,
                                          EnumTipoSerializacao.Javascript,
                                          culture,
                                          true,
                                          false);
            }
            catch
            {
                return json;
            }
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


    internal class ResolverReferencia : IReferenceResolver
    {
        public object ResolveReference(object context, string reference)
        {
            throw new NotImplementedException();
        }

        public string GetReference(object context, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsReferenced(object context, object value)
        {
            throw new NotImplementedException();
        }

        public void AddReference(object context, string reference, object value)
        {
            throw new NotImplementedException();
        }

    }

    public enum EnumTipoSerializacao
    {
        Javascript = 1,
        DotNet = 2

    }

}