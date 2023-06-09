﻿using Core.Common.Model.Transaccion.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Model.ExcepcionServicio
{
    public class ExcepcionServicio : System.Exception
    {
        public ExcepcionServicio(Exception exception)
        {
            Mensaje = new Mensaje(exception.HResult, exception.Message, true, exception.Source,exception.HResult);
        }

        public ExcepcionServicio(int codigoInternoError, string mensajePersonalizado = "")
        {
            CodigoInternoError = codigoInternoError;
            MensajePersonalizado = mensajePersonalizado;
        }

        public string MensajePersonalizado { get; set; }
        public Mensaje Mensaje { get; set; }
        public int CodigoInternoError { get; set; }
    }
}
