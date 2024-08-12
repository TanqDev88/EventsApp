using System;
using System.Collections.Generic;
using System.Text;
using Ticketera.Localization;
using Volo.Abp.Application.Services;

namespace Ticketera;

/* Inherit your application services from this class.
 */
public abstract class TicketeraAppService : ApplicationService
{
    protected TicketeraAppService()
    {
        LocalizationResource = typeof(TicketeraResource);
    }
}
