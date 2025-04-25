using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PointOfSaleSystem.Models;
using PointOfSaleSystem.Services;
using PointOfSaleSystem.Utils;

namespace PointOfSaleSystem.Views.ViewModels
{
    public class TableViewModel
    {
        public FullObservableCollection<Table> Tables { get; set; } = new FullObservableCollection<Table>();

        public TableViewModel()
        {
            var dao = Services.Services.GetKeyedSingleton<IDao>();
            var tableRepository = dao.Tables;
            Tables = new FullObservableCollection<Table>(tableRepository.GetAll());
        }
    }
}
