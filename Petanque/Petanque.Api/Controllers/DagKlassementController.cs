﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;

namespace Petanque.Api.Controllers {
    [Route("api/dagklassementen")]
    [ApiController]
    public class DagKlassementController(IDagKlassementService service, ISpeeldagService Sservice) : ControllerBase {
        [HttpGet("{id}")]
        public ActionResult<IEnumerable<DagKlassementResponseContract>> Get([FromRoute] int id) {
            var dagklassement = service.GetById(id);
            if (dagklassement is null) return NotFound();
            return Ok(dagklassement);
        }

        [HttpPost]
        public ActionResult<DagKlassementResponseContract> Create(
            [FromBody] DagKlassementRequestContract request) {
            var created = service.Create(request);
            return CreatedAtAction(nameof(Get), new { id = created.SpelerId }, created);
        }
        [HttpPost("{id}")]
        public ActionResult<IEnumerable<DagKlassementResponseContract>> CreateDagKlassement([FromRoute] int id)
        {
            var speeldagscores = Sservice.GetById(id);
            var dagklassementen = service.CreateDagKlassementen(speeldagscores, id);
            return Ok(dagklassementen);
        }
    }
}
