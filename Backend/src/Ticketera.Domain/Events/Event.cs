using System;
using System.Collections.Generic;
using System.Reflection;
using Ticketera.Entities;
using Ticketera.Enum;
using Ticketera.Extensions;
using Volo.Abp.Data;

namespace Ticketera.Events
{
    public class Event : BaseAudited, IHasExtraProperties
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;        
        public EventType EventType { get; set; } = EventType.Public;
        public bool IsMain { get; set; }
        public string Place { get; set; } = string.Empty;
        public string Code {  get; set; } = string.Empty;
        public string BgColor { get; set; } = string.Empty;
        public EventStatus EventStatus { get; set; } = EventStatus.Available;
        public ExtraPropertyDictionary ExtraProperties { get; } = new ExtraPropertyDictionary();

        #region Enumerable
        public virtual IEnumerable<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual IEnumerable<EventDate> EventDates { get; set; } = new List<EventDate>();
        public virtual IEnumerable<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
        public virtual IEnumerable<FileAttachment> FileAttachments { get; set; } = new List<FileAttachment>();
        public virtual IEnumerable<EventProviderPayment> EventProviders { get; set; } = new List<EventProviderPayment>();
        #endregion

        #region Properties Not Mapped        

        public IList<PriceModel> Prices
        {
            get => this.GetPropertyList<IList<PriceModel>>("Prices");
            set => this.SetPropertyList("Prices", value);
        }

        public int SaleForPerson 
        {
            get => this.GetProperty<int>("SaleForPerson");
            set => this.SetProperty("SaleForPerson", value);
        }

        public bool IsActive
        {
            get => this.GetProperty<bool>("IsActive");
            set => this.SetProperty("IsActive", value);
        }

        public string BackgroundJobId
        {
            get => this.GetProperty<string>("BackgroundJobId");
            set => this.SetProperty("BackgroundJobId", value);
        }

        #endregion


        public void SetCreatorIdExternally(Guid? id)
        {
            // Obtener la propiedad CreatorId de la clase base
            var propertyInfo = typeof(BaseAudited).GetProperty("CreatorId", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            // Verificar si la propiedad existe
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                // Establecer el valor de la propiedad
                propertyInfo.SetValue(this, id);
            }
        }
    }
}
