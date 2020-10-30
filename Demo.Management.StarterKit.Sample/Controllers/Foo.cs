using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit;
using System;

namespace Demo.Management.StarterKit.Sample.Controllers
{
    [Produces("application/json")]
	[Route("api/v1/AssignmentGroup/RuleMapping")]
	public class Foo : Controller
	{
		private readonly IUserContext _userContext;

		public Foo(IUserContext userContext)
		{
			_userContext = userContext;
		}
		[HttpPost]
		[Authorize]
		public IActionResult Bar(AsssignmentGroupRuleMappingrequestModel AsssignmentGroupRuleMappingrequestModel)
		{
			var userId = _userContext.Username;
			//DO SOMETHING!!!
			return Ok(new
			{
				RuleMappingId = new Random().Next(0, 10000),
				AssignmentGroup = AsssignmentGroupRuleMappingrequestModel.AssignmentGroup,
				RuleId = AsssignmentGroupRuleMappingrequestModel.RuleId,
				CreatedByUser = userId
			});
		}
	}

	/// <summary>
	/// This is a nice object
	/// </summary>
	public class AsssignmentGroupRuleMappingrequestModel
	{
		/// <summary>
		/// Some property for assignment Group
		/// </summary>
		public string AssignmentGroup { get; set; }

		/// <summary>
		/// RuleId assignment Group
		/// </summary>
		public int RuleId { get; set; }
	}
}
