using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snebur.Dominio;
using Newtonsoft.Json;
using Snebur.Utilidade;

namespace Snebur.Comunicacao
{

    public class JsonUtil
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
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize

        };

        private static readonly JsonSerializerSettings ConfiguracoesSerializarDotNet = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ReferenceLoopHandling = ReferenceLoopHandling.Serialize
        };

        public static T Deserializar<T>(string json)
        {
            return (T)Deserializar(json, typeof(T));
        }

        public static object Deserializar(string json, Type tipo)
        {
            try
            {
                var objeto = JsonConvert.DeserializeObject(json, tipo, JsonUtil.ConfiguracoesDeserializar);
                using (NormalizarDeserializacao normalizar = new NormalizarDeserializacao(objeto))
                {
                    normalizar.Normalizar();
                }
                return objeto;

            }
            catch (Exception ex)
            {
                throw new ErroDeserializarContrato(json, ex);
            }
        }
        public static string Serializar(object objeto)
        {
            return JsonUtil.Serializar(objeto, false);
        }

        public static string Serializar(object objeto, bool javascript)
        {
            return JsonUtil.Serializar(objeto, javascript, System.Diagnostics.Debugger.IsAttached);

        }
        public static string Serializar(object objeto, bool javascript, bool identar)
        {
            ErroUtil.ValidarReferenciaNula(objeto, nameof(objeto));
            var tipo = objeto.GetType();
            try
            {
                if (objeto != null && objeto is BaseDominio)
                {
                    List<Entidade> bases = new List<Entidade>();

                    using (var preparar = new PrepararSerializacao())
                    {
                        preparar.Preparar(objeto);
                    }
                }

                var formatacaoJson = Formatting.None;

                if (identar)
                {
                    formatacaoJson = Formatting.Indented;
                }

                var configuracaoSerializacao = javascript ? JsonUtil.ConfiguracoesSerializarJavascript : JsonUtil.ConfiguracoesSerializarDotNet;
                var json = JsonConvert.SerializeObject(objeto, formatacaoJson, configuracaoSerializacao);
                return json;


            }
            catch (Exception ex)
            {
                throw new ErroSerializarPacote(tipo, ex);
            }
        }


    }

}
