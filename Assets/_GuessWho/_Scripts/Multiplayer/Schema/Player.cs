// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 4.0.19
// 

using Colyseus.Schema;
#if UNITY_5_3_OR_NEWER
using UnityEngine.Scripting;
#endif

public partial class Player : Schema {
#if UNITY_5_3_OR_NEWER
[Preserve]
#endif
public Player() { }
	[Type(0, "string")]
	public string username = default(string);

	[Type(1, "uint8")]
	public byte secretCardId = default(byte);

	[Type(2, "uint8")]
	public byte cardsActive = default(byte);

	[Type(3, "boolean")]
	public bool isReady = default(bool);
}

