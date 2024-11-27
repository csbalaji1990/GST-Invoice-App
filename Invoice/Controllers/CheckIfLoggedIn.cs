using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace Invoice.Controllers
{
    public class CheckIfLoggedIn : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ctx = HttpContext.Current;

            if (ctx.Session["companyId"] == null || ctx.Session["companyName"] == null || ctx.Session["companyLogo"] == null || ctx.Session["companyState"] == null)
                filterContext.Result = new RedirectResult("~/Home/Login");
        }
    }

    public static class Helper
    {
        public static string Inr(float Number)
        {
            var result = decimal.Parse(Number.ToString(), System.Globalization.NumberStyles.Any);

            return String.Format(new System.Globalization.CultureInfo("hi-IN"), "{0:c}", decimal.Parse(result.ToString(), System.Globalization.CultureInfo.InvariantCulture)).Replace("₹ ", "");
        }

        public static string ToInr(this float Number)
        {
            var result = decimal.Parse(Number.ToString(), System.Globalization.NumberStyles.Any);

            return String.Format(new System.Globalization.CultureInfo("hi-IN"), "{0:c}", decimal.Parse(result.ToString(), System.Globalization.CultureInfo.InvariantCulture)).Replace("₹ ", "");
        }

        public static string ToWords(this string Number)
        {
            var result = changeToWords(Number, true);
            return result.Replace("Zero  Only", "Only");
        }

        private static string changeToWords(string Number, bool isCurrency)
        {
            string val = "", wholeNo = Number, points = "", andStr = "", pointStr = "";

            var endStr = (isCurrency) ? ("Only") : ("");

            try
            {
                var decimalPlace = Number.IndexOf(".");
                
                if (decimalPlace > 0)
                {
                    wholeNo = Number.Substring(0, decimalPlace);

                    points = Number.Substring(decimalPlace + 1);

                    if (int.Parse(points) > 0)
                    {
                        andStr = (isCurrency) ? ("and") : ("point");
                        endStr = (isCurrency) ? ("Paisa " + endStr) : ("");

                        pointStr = translateCents(points);
                    }
                }

                val = String.Format("{0} {1}{2} {3}", translateWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch {
            }

            return val;
        }

        private static string translateWholeNumber(string Number)
        {
            var word = "";

            try
            {
                var beginsZero = false;

                var isDone = false;
                var dblAmt = double.Parse(Number);

                if (dblAmt > 0)
                {
                    beginsZero = Number.StartsWith("0");

                    var numDigits = Number.Length;

                    var pos = 0;

                    var place = "";

                    switch (numDigits)
                    {
                        case 1:
                            word = ones(Number);
                            isDone = true;
                            break;
                        case 2:
                            word = tens(Number);
                            isDone = true;
                            break;
                        case 3:
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4:
                        case 5:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 6:
                        case 7:
                            pos = (numDigits % 6) + 1;
                            place = " Lakh ";
                            break;
                        case 8:
                        case 9:
                            pos = (numDigits % 8) + 1;
                            place = " Crore ";
                            break;
                        case 10:
                        case 11:
                            pos = (numDigits % 10) + 1;
                            place = " Arab ";
                            break;
                        default:
                            isDone = true;
                            break;
                    }

                    if (!isDone)
                    {
                        word = translateWholeNumber(Number.Substring(0, pos)) + place + translateWholeNumber(Number.Substring(pos));

                        if (beginsZero)
                            word = " and " + word.Trim();
                    }

                    if (word.Trim().Equals(place.Trim()))
                        word = "";
                }
                else
                    word = " Zero ";

                var result = word.Trim();
                result = result.Replace("and Hundred", "");
                result = result.Replace("and Thousand", "");
                result = result.Replace("and Lakh", "");
                result = result.Replace("and Crore", "");
                result = result.Replace(" and ", " ");
                word = result;
            }
            catch {
            }
            
            return word.Trim();
        }

        private static string tens(string Digit)
        {
            var digt = int.Parse(Digit);

            string name = null;
            
            switch (digt)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Fourty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (digt > 0)
                        name = tens(Digit.Substring(0, 1) + "0") + " " + ones(Digit.Substring(1));
                    break;
            }

            return name;
        }

        private static string ones(string Digit)
        {
            var digt = int.Parse(Digit);

            var name = "";

            switch (digt)
            {
                case 0:
                    name = "Zero";
                    break;
                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }

            return name;
        }

        private static string translateCents(string Cents)
        {
            string cts = "", digit = "", engOne = "";

            for (int i = 0; i < Cents.Length; i++)
            {
                digit = Cents[i].ToString();

                if (digit.Equals("0"))
                    engOne = "Zero";
                else
                    engOne = ones(digit);

                cts += " " + engOne;

                if (i == 1)
                {
                    if (int.Parse(Cents) > 9 && int.Parse(Cents) < 21)
                        cts = " " + tens(Cents);
                    else
                    {
                        digit = Cents[0].ToString();
                        cts = " " + tens(digit + "0");
                        digit = Cents[1].ToString();
                        cts += " " + ones(digit);
                    }
                }
            }

            return cts;
        }
    }
}