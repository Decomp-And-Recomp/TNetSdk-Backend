namespace TNet;

internal enum Version
{
    /// <summary>
    /// CoM:Z, TLCK
    /// </summary>
    Gen1 = 0,
    /// <summary>
    /// CoM:DH
    /// </summary>
    Gen2 = 1,
    /// <summary>
    /// CoM:Infinity
    /// </summary>
    Gen3 = 2
}

/*
 * The difference between gens is quite big
 * On Gen 1 server has to send the JoinRoomRes to the room's creator after creating a room.
 * But on Gen 2 Client automatically sends a join request to the room after Create Room Response. 
 * 
 * Gen 3 is a compleatly different story, it has like 15 additional commands
 * and some implementations that dont exist in most of the games.
 */