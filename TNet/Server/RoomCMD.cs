namespace TNet.Server;

internal enum RoomCMD
{
	none = 0, // N/A
	drag_list = 1, // Implemented
	drag_list_res = 2, // Implemented Experemental
    create = 3, // Implemented
    create_res = 4, // Implemented Experemental
    destroy = 5,
	destroy_res = 6, // Implemented
    destroy_notify = 7, // Implemented
    join = 8, // Implemented Experemental
    join_res = 9, // Implemented Experemental
    join_notify = 10, // Implemented Experemental
    leave = 11, // Implemented Experemental
    leave_notify = 12, // Implemented Experemental
    kick_user = 13,
	kick_user_notify = 14,
	rename = 15,
	rename_notify = 16,
	set_var = 17, // Implemented Experemental
    var_notify = 18, // Implemented Experemental
    set_user_var = 19, // Implemented Experemental
    user_var_notify = 20, // Implemented Experemental
    set_user_status = 21, // Implemented Experemental
    user_status_notify = 22,
	send_msg = 23, // Unused?
	broadcast_msg = 24, // Implemented
    msg_notify = 25, // Implemented
    lock_req = 26, // Implemented Experemental
    lock_res = 27, // Implemented
    unlock_req = 28,
	unlock_res = 29,
	start = 30, // Implemented Experemental
    start_notify = 31, // Implemented
    creater_notify = 32, // Implemented Experemental
    set_create_param = 33,
	create_param_change = 34
}