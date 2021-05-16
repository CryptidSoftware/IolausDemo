using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Iolaus;
using System.Text.Json;
using System.Reactive.Linq;

namespace IolausDemo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly Router _router;

        [BindProperty]
        public int NumDice { get; set; }
        [BindProperty]
        public int SidesPerDie { get; set; }
        [BindProperty]
        public bool Stats { get; set; }

        public string Reply { get; set; }

        public IndexModel(ILogger<IndexModel> logger, Router router)
        {
            _logger = logger;
            _router = router;
        }

        public void OnGet()
        {

        }

        public async Task OnPost()
        {
            var message = new {type = "dice", cmd = "roll", NumDice, SidesPerDie, Stats};
            Reply = await Message.FromObject(message)
                .Match(
                    Some: async (m) => 
                    {
                        var send = _router.GetFunc(m);
                        var stream = send(m);
                        var firstResponse = await stream.FirstAsync();
                        return firstResponse.Match(
                            Some: (response) => response.ToString(),
                            None: () => "No Reply"
                        );
                    },
                    None: () => Task.FromResult("Parse Error")
                );
        }
    }
}
