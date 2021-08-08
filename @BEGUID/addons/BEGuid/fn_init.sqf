/*

*/
private _id = addMissionEventHandler ["PlayerConnected", BEGUID_fnc_onConnected, ["BEGUID_HashMap"]];

diag_log format ["BEGUID PlayerConnected EVH Loaded: ID: %1",_id];