using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class CustomAuthorizationPolicy : IAuthorizationPolicy
    {
        // Geter
        public string Id
        {
            get;
        }

        // Konstruktor
        public CustomAuthorizationPolicy()
        {
            Id = Guid.NewGuid().ToString();
        }

        // Izdavač
        public ClaimSet Issuer
        {
            get { return ClaimSet.System; }
        }

        // Evaluacija
        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            // Pokušaj dobavljanja identiteta
            if (!evaluationContext.Properties.TryGetValue("Identities", out object list))
            {
                return false;
            }

            // Ako je lista identiteta prazna ili null
            IList<IIdentity> identities = list as IList<IIdentity>;
            if (list == null || identities.Count <= 0)
            {
                return false;
            }

            // Pravljenje novog principala
            evaluationContext.Properties["Principal"] = new CustomPrincipal((WindowsIdentity)identities[0]);
            return true;
        }
    }
}
