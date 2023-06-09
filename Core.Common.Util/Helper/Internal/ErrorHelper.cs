﻿using Core.Common.DataAccess.Procesos.API;
using Core.Common.DataAccess.Procesos.Errores;
using Core.Common.Model.Transaccion.Base;

namespace Core.Common.Util.Helper.Internal
{
    public static class ErrorHelper
    {
        /// <summary>
        /// Metodo para obtener códigos internos de respuesta
        /// </summary>
        /// <param name="codigoInternoRespuesta"></param>
        /// <returns></returns>
        public static Mensaje ObtenerMensajeRespuesta(int codigoInternoRespuesta, string mensajePersonalizado="")
        {
            var errorMicroservicio = ObtenerErrorMicroservicioDAL.Execute(codigoInternoRespuesta);
            var mensaje = new Mensaje(errorMicroservicio.CodigoInterno, string.Format(errorMicroservicio.MensajeError,mensajePersonalizado), true, errorMicroservicio.Modulo, errorMicroservicio.CodigoInterno);
            return mensaje;
        }      
    }
}
