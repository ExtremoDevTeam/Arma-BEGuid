/*

*/
private _HashMap = uiNamespace getVariable ["BEGUID_HashMap",createHashMap];

if(typeName _this isEqualTo "ARRAY")exitWith{
	_this apply {
		if(_x in _HashMap)then{
			_HashMap get _x
		}else{
			'BEGuid' callExtension format["get:%1",_x]
		}
	}
};

if(_this in _HashMap)then{
	_HashMap get _this
}else{
	'BEGuid' callExtension format["get:%1",_this]
}