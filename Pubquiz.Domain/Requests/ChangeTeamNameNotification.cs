using System;
using System.Threading.Tasks;

namespace Pubquiz.Domain.Requests
{
    public class ChangeTeamNameNotification : Notification
    {
        private readonly Guid _teamId;
        private readonly string _newName;

        public ChangeTeamNameNotification(Guid teamId, string newName)
        {
            _teamId = teamId;
            _newName = newName;
        }
        protected override Task DoExecute()
        {
            // check team exists
            
            // set new name
            throw new System.NotImplementedException();
        }
    }
}