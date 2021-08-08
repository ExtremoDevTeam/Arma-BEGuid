/*

*/

params [
	["_id",-100,[0]],		// Number - is the unique DirectPlay ID. Quite useless as the number is too big for in-built string representation and gets rounded. It is also the same id used for user placed markers.
	["_SteamID","",[""]],	// String - is getPlayerUID of the joining player. In Arma 3 it is also the same as Steam ID.
	["_name","",[""]],		// String - is profileName of the joining player.
	["_jip",false,[false]], // Boolean - is a flag that indicates whether or not the player joined after the mission has started (Joined In Progress). true when the player is JIP, otherwise false. (since Arma 3 v1.49)
	["_owner",-100,[0]],	// Number - is owner id of the joining player. Can be used for kick or ban purposes or just for publicVariableClient. (since Arma 3 v1.49) 
	["_idstr","",[""]],		// String - same as _id but in string format, so could be exactly compared to user marker ids. (since Arma 3 v1.95) 
	["_customArgs",[]] 		// User-Defined - custom passed args (since Arma 3 v2.03) 
];

private _HashMap = uiNamespace getVariable ["BEGUID_HashMap",createHashMap];
private _BEGuid = _HashMap getOrDefault [_SteamID, "NONE"];
if(_BEGuid isEqualTo "NONE")then{
	_BEGuid = 'BEGuid' callExtension format["get:%1",_SteamID];
	_HashMap set [_SteamID, _BEGuid];
	uiNamespace setVariable ["BEGUID_HashMap",_HashMap];
};

diag_log format ["PlayerConnected: %1#%2 ~ %3 ~ %4",_name, _id, _BEGuid, _SteamID]; 