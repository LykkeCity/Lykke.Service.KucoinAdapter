﻿using System;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models.Fails
{
    public class InvalidInstrumentException : Exception
    {
        public InvalidInstrumentException()
        {
        }

        public InvalidInstrumentException(string message)
            : base(message)
        {
        }

        public InvalidInstrumentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
