using System;
using System.Management.Automation;
using Dbg = System.Management.Automation;
using System.Management.Automation.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Globalization;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.PowerShell.Commands
{
    /// <summary>
    /// Allows the user to override the second
    /// </summary>
    [Cmdlet(VerbsCommon.New, "Password")]
    [OutputType(typeof(String))]
    public class NewPasswordCommand : Cmdlet
    {
        private string lowerset = "abcdefghijklmnopqrstuvwxyz";
        private string upperset = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private string numberset = "0123456789";
        private string symbolset = " !\"#$%&'()*+,-./:;<=>?@";
        private List<char> all = new List<char>();
        private List<char> excludes = new List<char>();
        private List<char> uppers = new List<char>();
        private List<char> lowers = new List<char>();
        private List<char> numbers = new List<char>();
        private List<char> symbols = new List<char>();
        private List<char> password = new List<char>();

        /// <summary>
        /// Allows the user to override the second
        /// </summary>
        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true)]
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        private int length = 8;

        /// <summary>
        /// Allows the user to override the second
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public int Upper
        {
            get { return upper; }
            set { upper = value; }
        }
        private int upper = 1;

        /// <summary>
        /// Allows the user to override the second
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public int Lower
        {
            get { return lower; }
            set { lower = value; }
        }
        private int lower = 1;

        /// <summary>
        /// Allows the user to override the second
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public int Number
        {
            get { return number; }
            set { number = value; }
        }
        private int number = 1;

        /// <summary>
        /// Allows the user to override the second
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public int Symbol
        {
            get { return symbol; }
            set { symbol = value; }
        }
        private int symbol = 1;

        /// <summary>
        /// Allows the user to override the second
        /// </summary>
        [Parameter(ValueFromPipelineByPropertyName = true)]
        public string Exclude
        {
            get { return exclude; }
            set { exclude = value; }
        }
        private string exclude;

        /// <summary>
        /// returns a string
        /// </summary>
        protected override void ProcessRecord()
        {

            var minlength = upper + lower + number + symbol;
            if(length < minlength){
                ErrorRecord er = new ErrorRecord(e, "FormatException", ErrorCategory.InvalidType, length);
                ThrowTerminatingError(er);
            }

            // generate character arrays
            uppers.AddRange(upperset.ToCharArray());
            lowers.AddRange(lowerset.ToCharArray());
            numbers.AddRange(numberset.ToCharArray());
            symbols.AddRange(symbolset.ToCharArray());

            // exclude characters
            if(!String.IsNullOrEmpty(exclude)){
                foreach(var c in exclude.ToCharArray()){
                    uppers.Remove(c);
                    lowers.Remove(c);
                    numbers.Remove(c);
                    symbols.Remove(c);
                }
            }

            // get remaining cahracters, all classes
            all.AddRange(uppers);
            all.AddRange(lowers);
            all.AddRange(numbers);
            all.AddRange(symbols);

            // get random cahracter from each class
            password.AddRange(GetRandomCharacters(uppers, upper));
            password.AddRange(GetRandomCharacters(lowers, lower));
            password.AddRange(GetRandomCharacters(numbers, number));
            password.AddRange(GetRandomCharacters(symbols, symbol));

            var remainder = length - password.Count;
            if(remainder != 0){
                password.AddRange(GetRandomCharacters(all, remainder));
            }

            password.Shuffle();
            var sb = new System.Text.StringBuilder();
            foreach(var c in password){ sb.Append(c); }
            var result = sb.ToString();
            WriteObject(result);
        }

        private string GetRandomCharacters(List<char> input, int count){
            var rnd = new Random();
            var sb = new System.Text.StringBuilder();
            for(int r = 0; r < count; r++){
                sb.Append(input[rnd.Next(input.Count)]);
            }
            return sb.ToString();
        }
    }
    /// <summary>
    /// Allows the user to override the second
    /// </summary>
    public static class Extension {
        /// <summary>
        /// Allows the user to override the second
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}