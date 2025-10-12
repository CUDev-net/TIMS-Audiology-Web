using System.Threading.Tasks;
using TIMS_X.Core.Domain;
using TIMS_X.DAL.DAL.UoWs;

namespace TIMS_X.BLL.Repositories;

public interface IMessageSettingsRepository
{
	Task<MessageSettings> Add(MessageSettings messageSettings);
	void Delete(int id);
	Task<MessageSettings> Get();
	Task<MessageSettings> Update(MessageSettings messageSettings);
}

public class MessageSettingsRepository : IMessageSettingsRepository
{
	private readonly IMessageSettingsUnitOfWork _messageSettingsUnitOfWork;

	public MessageSettingsRepository(IMessageSettingsUnitOfWork messageSettingsUnitOfWork)
	{
		_messageSettingsUnitOfWork = messageSettingsUnitOfWork;
	}

	public async Task<MessageSettings> Add(MessageSettings messageSettings)
	{
		return await _messageSettingsUnitOfWork.Add(messageSettings);
	}

	public void Delete(int id)
	{
		_messageSettingsUnitOfWork.Delete(id);
	}

	public async Task<MessageSettings> Get()
	{
		return await _messageSettingsUnitOfWork.GetMessageSettings();
	}

	public async Task<MessageSettings> Update(MessageSettings messageSettings)
	{
		return await _messageSettingsUnitOfWork.Update(messageSettings);
	}
}