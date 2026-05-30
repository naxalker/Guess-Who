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

public partial class GuessWhoState : Schema {
#if UNITY_5_3_OR_NEWER
[Preserve]
#endif
public GuessWhoState() { }
	[Type(0, "map", typeof(MapSchema<Player>))]
	public MapSchema<Player> players = null;

	[Type(1, "string")]
	public string phase = default(string);

	[Type(2, "string")]
	public string currentTurn = default(string);

	[Type(3, "uint8")]
	public byte countdown = default(byte);
}

