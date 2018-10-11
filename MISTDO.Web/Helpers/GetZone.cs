using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MISTDO.Web.Helpers
{
    public static class GetZone
    {
        public static string Zone(string state)
        {

            if (!String.IsNullOrEmpty(state))
            {
                if (state.Contains(States.Benue) || state.Contains(States.Kogi) || state.Contains(States.Kwara) || state.Contains(States.Nasarawa) || state.Contains(States.Niger) || state.Contains(States.Plateau) || state.Contains(States.FCT))
                {
                    return "NC";

                }
                if (state.Contains(States.Adamawa) || state.Contains(States.Bauchi) || state.Contains(States.Borno) || state.Contains(States.Gombe) || state.Contains(States.Taraba) || state.Contains(States.Yobe))
                {
                    return "NE";

                }
                if (state.Contains(States.Jigawa) || state.Contains(States.Kaduna) || state.Contains(States.Kano) || state.Contains(States.Katsina) || state.Contains(States.Kebbi) || state.Contains(States.Sokoto) || state.Contains(States.Zamfara))
                {
                    return "NW";

                }
                if (state.Contains(States.Abia) || state.Contains(States.Anambra) || state.Contains(States.Ebonyi) || state.Contains(States.Enugu) || state.Contains(States.Imo))
                {
                    return "SE";

                }
                if (state.Contains(States.AkwaIbom) || state.Contains(States.Bayelsa) || state.Contains(States.CrossRiver) || state.Contains(States.Delta) || state.Contains(States.Edo) || state.Contains(States.Rivers))
                {
                    return "SS";

                }
                if (state.Contains(States.Ekiti) || state.Contains(States.Lagos) || state.Contains(States.Ogun) || state.Contains(States.Ondo) || state.Contains(States.Osun) || state.Contains(States.Oyo))
                {
                    return "SW";

                }
            }
            return null;
        }

    }
    public static class States
    {

        public static string Abia = "Abia";
        public static string Adamawa = "Adamawa State";
        public static string AkwaIbom = "Akwa Ibom";
        public static string Anambra = "Anambra";
        public static string Bauchi = "Bauchi";
        public static string Bayelsa = "Bayelsa";
        public static string Benue = "Benue";
        public static string Borno = "Borno";
        public static string CrossRiver = "Cross River";

        public static string Delta = "Delta";
        public static string Ebonyi = "Ebonyi";
        public static string Enugu = "Enugu";
        public static string Edo = "Edo";
        public static string Ekiti = "Ekiti";
        public static string Gombe = "Gombe";
        public static string Imo = "Imo";
        public static string Jigawa = "Jigawa";
        public static string Kaduna = "Kaduna";
        public static string Kano = "Kano";
        public static string Katsina = "Katsina";
        public static string Kebbi = "Kebbi";
        public static string Kogi = "Kogi";
        public static string Kwara = "Kwara";
        public static string Lagos = "Lagos";
        public static string Nasarawa = "Nasarawa";
        public static string Niger = "Niger";
        public static string Ogun = "Ogun";
        public static string Ondo = "Ondo";
        public static string Osun = "Osun";
        public static string Oyo = "Oyo";
        public static string Plateau = "Plateau";
        public static string Rivers = "Rivers";
        public static string Sokoto = "Sokoto";
        public static string Taraba = "Taraba";
        public static string Yobe = "Yobe";
        public static string Zamfara = "Zamfara";
        public static string FCT = "FCT";


    }
}
