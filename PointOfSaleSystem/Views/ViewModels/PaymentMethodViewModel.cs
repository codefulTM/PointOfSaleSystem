using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;
using PointOfSaleSystem.Services;
using PointOfSaleSystem.Utils;

namespace PointOfSaleSystem.Views.ViewModels
{
    public class PaymentMethodViewModel
    {
        public FullObservableCollection<PaymentMethod> BankPaymentMethods { get; set; }
        public FullObservableCollection<PaymentMethod> MomoPaymentMethods { get; set; }
        public PaymentMethodViewModel()
        {
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var paymentMethodRepository = dao.PaymentMethods;

            BankPaymentMethods = new FullObservableCollection<PaymentMethod>(paymentMethodRepository.GetAll().Where(x => x.Type == "bank") ?? new List<PaymentMethod>());
            MomoPaymentMethods = new FullObservableCollection<PaymentMethod>(paymentMethodRepository.GetAll().Where(x => x.Type == "momo") ?? new List<PaymentMethod>());
        }
    }
}
