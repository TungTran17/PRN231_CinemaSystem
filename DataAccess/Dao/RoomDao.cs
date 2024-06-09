using BussinessObject.Models;

namespace DataAccess.Dao
{
    public class RoomDao
    {
        private static RoomDao instance = null;
        private static readonly object roomDaoLock = new object();

        public static RoomDao GetRoomDAO
        {
            get
            {
                lock (roomDaoLock)
                {
                    if (instance == null)
                    {
                        instance = new RoomDao();
                    }
                    return instance;
                }
            }
        }

        public List<Room> getListRoom()
        {
            var rooms = new List<Room>();
            try
            {
                using var context = new CinemaSystemContext();
                rooms = context.Rooms.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return rooms;
        }

        public Room getRoomById(int roomId)
        {
            var room = new Room();
            try
            {
                using var context = new CinemaSystemContext();
                room = context.Rooms.Find(roomId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return room;
        }
    }
}
