using Ticketera.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Ticketera.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class TicketeraController : AbpControllerBase
{
    protected TicketeraController()
    {
        LocalizationResource = typeof(TicketeraResource);
    }
}
