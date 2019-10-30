using System.ComponentModel;
using UserApplication.ViewModels;

namespace ImageFilters.ViewModel
{
    public class AdaptiveTreshHoldeParamViewModel : BaseViewModel, IDataErrorInfo
    {
        private int paramS;
        private float paramT;

        public AdaptiveTreshHoldeParamViewModel()
        {
            ParamS = "41";
            ParamT = "0.15";
        }

        public string ParamS
        {
            get => paramS.ToString();
            set
            {
                int.TryParse(value, out paramS);
                RaisePropertyChanged(() => ParamS);
            }
        }

        public string ParamT
        {
            get => string.Format("{0:0.00}", paramT);
            set
            {
                float.TryParse(value, out paramT);
                RaisePropertyChanged(() => ParamT);
            }
        }

        public string this[string property]
        {
            get
            {
                string result = null;
                if (property == "ParamS")
                {
                    if (string.IsNullOrEmpty(ParamS))
                        result = "Pole wymagane. Wprowadz Rozmiar";

                }

                if (property == "ParamT")
                {
                    if (string.IsNullOrEmpty(ParamT))
                        result = "Pole wymagane. Wprowadz Rozmiar";

                    if (paramT <= 0.0 || paramT >= 1.0)
                        result = "Wartość parametru musi być z przedział (0.0,1.0)";
                }

                return result;
            }

        }
        public string Error
        {
            get
            {
                return string.Empty;
            }


        }

       
    }
}
