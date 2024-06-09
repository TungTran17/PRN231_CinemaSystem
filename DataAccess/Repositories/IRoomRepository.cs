using BussinessObject.Models;

namespace DataAccess.Repositories
{
    public interface IRoomRepository
    {
        List<Room> getListRooms();
        public Room getRoomById(int roomId);
    }
}
