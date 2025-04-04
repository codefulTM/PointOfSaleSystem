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
        public FullObservableCollection<PaymentMethod> PaymentMethods { get; set; }

        public PaymentMethodViewModel()
        {
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var paymentMethodRepository = dao.PaymentMethods;
            PaymentMethods = new FullObservableCollection<PaymentMethod>(paymentMethodRepository.GetAll());
        }
    }
}
