/*

*/

if(typeName _this isEqualTo "ARRAY")exitWith{
	_this apply {
		'BEGuid' callExtension format["get:%1",_x]
	}
};

'BEGuid' callExtension format["get:%1",_this]