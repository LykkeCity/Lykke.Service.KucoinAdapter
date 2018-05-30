using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Lykke.Service.KucoinAdapter.Services.RestApi.Models.Fails
{
    public class NoBalanceException : Exception
    {
        public NoBalanceException()
        {
        }

        public NoBalanceException(string message)
            : base(message)
        {
        }

        public NoBalanceException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public static bool TryParse(string code, out NoBalanceException ex)
        {
            if (code == "NO_BALANCE")
            {
                ex = new NoBalanceException();
                return true;
            }

            ex = null;
            return false;
        }
    }

    public class VolumeTooSmallException: Exception
    {
        public VolumeTooSmallException()
        {
        }

        public VolumeTooSmallException(string message)
            : base(message)
        {
        }

        public VolumeTooSmallException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public static bool TryParse(string message, out VolumeTooSmallException ex)
        {
            var m = Regex.Match(message, @"^Min amount each order:(?<amount>.*)$");

            if (m.Success)
            {

                ex = new VolumeTooSmallException(message);
                return true;
            }

            ex = null;
            return false;
        }
    }
}
