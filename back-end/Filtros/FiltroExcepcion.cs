namespace back_end.Filtros
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class FiltroExcepcion: ExceptionFilterAttribute
    {
        private readonly ILogger<FiltroExcepcion> logger;

        public FiltroExcepcion(ILogger<FiltroExcepcion> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            this.logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
