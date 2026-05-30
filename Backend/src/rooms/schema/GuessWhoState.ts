import { Schema, MapSchema, type } from "@colyseus/schema";

export enum GamePhase {
    Waiting = "waiting",
    Preparing = "preparing",
    Countdown = "countdown",
    Playing = "playing",
    Finished = "finished",
}

export class Player extends Schema {
    @type("string") username: string = "";
    @type("uint8") secretCardId: number = -1;
    @type("uint8") cardsActive: number = 24;
    @type("boolean") isReady: boolean = false;
}

export class GuessWhoState extends Schema {
    @type({ map: Player }) players = new MapSchema<Player>();
    @type("string") phase: string = GamePhase.Waiting;
    @type("string") currentTurn: string = "";
    @type("uint8") countdown: number = 0;
}