import { Room, Client, validate, Delayed } from "colyseus";
import { z } from "zod";
import { GamePhase, GuessWhoState, Player } from "./schema/GuessWhoState.js";

const CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

export class GuessWhoRoom extends Room {
    LOBBY_CHANNEL = "guess_who_lobby";
    maxClients = 2;
    timer?: Delayed;
    state = new GuessWhoState();

    messages = {
        "update_active_cards": validate(z.object({
            count: z.number().min(0).max(24)
        }), (client, payload) => {
            const player = this.state.players.get(client.sessionId);
            if (player) {
                player.cardsActive = payload.count;
            }
        }),

        "select_card": validate(z.object({
            cardId: z.number()
        }), (client, payload) => {
            const player = this.state.players.get(client.sessionId);
            if (player && this.state.phase === GamePhase.Preparing) {
                player.secretCardId = payload.cardId;
            }
        }),

        "set_ready": validate(z.object({
            isReady: z.boolean()
        }), (client, payload) => {
            const player = this.state.players.get(client.sessionId);
            if (player &&
                this.state.phase === GamePhase.Preparing &&
                player.secretCardId !== -1
            ) {
                player.isReady = payload.isReady;
                this.checkBothReady();
            }
        }),

        "chat_message": validate(z.object({
            text: z.string().min(1).max(256)
        }), (client, payload) => {
            this.broadcast("chat_message", {
                senderId: client.sessionId,
                text: payload.text
            });
        }),

        "end_turn": (client: Client) => {
            if (this.state.phase === "playing" && this.state.currentTurn === client.sessionId) {
                this.switchTurn();
            }
        },

        // "guess": validate(z.object({
        //     guessedCardId: z.number()
        // }), (client, payload) => {
        //     if (this.state.status === "playing" && this.state.currentTurn === client.sessionId) {
        //         const myId = client.sessionId;
        //         const opponentId = Array.from(this.state.players.keys()).find(id => id !== myId);
        //         const opponent = this.state.players.get(opponentId!);

        //         if (opponent) {
        //             if (opponent.secretCardId === payload.guessedCardId) {
        //                 this.state.winner = myId;
        //             } else {
        //                 this.state.winner = opponentId!;
        //             }
        //             this.state.status = "finished";
        //         }
        //     }
        // })
    };

    generateRoomIdSingle(): string {
        let result = "";
        for (let i = 0; i < 6; i++) {
            result += CHARS.charAt(Math.floor(Math.random() * CHARS.length));
        }
        return result;
    }

    async generateRoomId(): Promise<string> {
        const currentIds = await this.presence.smembers(this.LOBBY_CHANNEL);
        let id: string;
        do {
            id = this.generateRoomIdSingle();
        } while (currentIds.includes(id));

        await this.presence.sadd(this.LOBBY_CHANNEL, id);
        return id;
    }

    async onCreate(options: any) {
        this.roomId = await this.generateRoomId();
    }

    async onDispose() {
        this.presence.srem(this.LOBBY_CHANNEL, this.roomId);
    }

    onJoin(client: Client, options: any) {
        const newPlayer = new Player();
        newPlayer.username = options.username || "Anonymous";
        this.state.players.set(client.sessionId, newPlayer);

        if (this.state.players.size === 2) {
            this.state.phase = GamePhase.Preparing;
        }
    }

    onLeave(client: Client, code?: number) {
        this.state.players.delete(client.sessionId);
        this.state.phase = "waiting";
    }

    private checkBothReady() {
        let allReady = true;
        this.state.players.forEach(p => { if (!p.isReady) allReady = false; });

        if (allReady && this.state.players.size === 2) {
            this.startCountdown();
        }
    }

    private startCountdown() {
        this.state.phase = GamePhase.Countdown;
        this.state.countdown = 3;

        this.timer = this.clock.setInterval(() => {
            this.state.countdown--;

            if (this.state.countdown <= 0) {
                this.timer?.clear();
                this.startGame();
            }
        }, 1000);
    }

    private startGame() {
        this.state.phase = GamePhase.Playing;
        this.state.currentTurn = Array.from(this.state.players.keys())[0];
    }

    private switchTurn() {
        const ids = Array.from(this.state.players.keys());
        this.state.currentTurn = ids.find(id => id !== this.state.currentTurn) || ids[0];
    }
}