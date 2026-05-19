using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Websitebanhang.Repository.Components
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        public FooterViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Contacts.FirstOrDefaultAsync());

    }
}
