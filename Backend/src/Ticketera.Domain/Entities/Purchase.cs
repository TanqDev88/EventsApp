using System;
using System.Collections.Generic;
using Ticketera.Enum;
using Volo.Abp.Data;

namespace Ticketera.Entities
{
    public class Purchase : BaseAudited, IHasExtraProperties
    {
        public string Code { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public PurchaseState State { get; set; } = PurchaseState.Pending;
        public ExtraPropertyDictionary ExtraProperties { get; } = new ExtraPropertyDictionary();

        public virtual IList<Ticket> Ticktes { get; set; } = new List<Ticket>();


        #region Not Mapped

        public string MercadoPagoPreference
        {
            get
            {
                return this.GetProperty<string>("MercadoPagoPreference") ?? "";
            }
            set
            {
                this.SetProperty("MercadoPagoPreference", value);
            }
        }

        public string MercadoPagoResponse
        {
            get
            {
                return this.GetProperty<string>("MercadoPagoResponse") ?? "";
            }
            set
            {
                this.SetProperty("MercadoPagoResponse", value);
            }
        }
        #endregion
    }
}
