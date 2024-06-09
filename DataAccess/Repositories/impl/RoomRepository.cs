using BussinessObject.Models;
using DataAccess.Dao;

namespace DataAccess.Repositories.impl
{
    public class RoomRepository : IRoomRepository
    {
        public List<Room> getListRooms() => RoomDao.GetRoomDAO.getListRoom();
        public Room getRoomById(int roomId) => RoomDao.GetRoomDAO.getRoomById(roomId);
    }
}
