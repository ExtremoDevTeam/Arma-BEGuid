/*

*/

if(typeName _this isEqualTo "ARRAY")exitWith{
	_this apply {
		[
			_x,
			'BEGuid' callExtension format["check:%1",_x]
		]
	}
};

[
	_this,
	'BEGuid' callExtension format["check:%1",_this]
]