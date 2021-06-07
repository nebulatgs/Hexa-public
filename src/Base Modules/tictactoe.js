const debug = new Vue({
    el: "#app",
    data: {
        board: [0, 0, 0, 0, 0, 0, 0, 0, 0],
        current: 1,
        player: 0,
        computer: 0,
        inGame: true,
        winner: 0,
        started: false
    },
    methods: {
        start(comp) {
            this.computer = comp;
            this.player = comp === 1 ? 2 : 1;
            this.started = true;
            if (comp === 1) {
                this.botMove();
            }
        },
        symbol(a) {
            switch (a) {
                case 0:
                    return "";
                case 1:
                    return "X";
                case 2:
                    return "O";
            }
        },
        mark(space) {
            if (this.board[space] === 0 && this.inGame) {
                this.board.splice(space, 1, this.current);
                if (this.check()) {
                    this.inGame = false;
                    this.winner = this.current;
                    return;
                }
                if (this.board.reduce((p, c) => p + c) === 13) {
                    this.inGame = false;
                    this.winner = -1;
                    return;
                }
                let next = this.current + 1;
                this.current = next === 2 ? 2 : 1;

                if (this.current === this.computer) this.botMove();
            }
        },
        check(grid, player) {
            if (!grid) grid = this.board;
            if (!player) player = this.current;
            const row = (p1, p2, p3) =>
                grid[p1] === player && grid[p2] === player && grid[p3] === player;

            return (
                // Rows
                row(0, 1, 2) ||
                row(3, 4, 5) ||
                row(6, 7, 8) ||
                // Columns
                row(0, 3, 6) ||
                row(1, 4, 7) ||
                row(2, 5, 8) ||
                // Diagonals
                row(0, 4, 8) ||
                row(2, 4, 6)
            );
        },
        restart() {
            this.board = [0, 0, 0, 0, 0, 0, 0, 0, 0];
            this.inGame = true;
            this.started = false;
            this.current = 1;
        },
        botMove() {
            let clone = () => {
                let grid = [];
                for (let i = 0; i < 9; i++) {
                    grid.push(this.board[i]);
                }
                return grid;
            };
            let ranMove = arr => {
                let possible = [];
                for (let i = 0; i < arr.length; i++) {
                    if (!this.board[arr[i]]) possible.push(arr[i]);
                }
                if (possible.length === 0) return null;
                else return possible[Math.floor(Math.random() * possible.length)];
            };

            // Can we win next move?
            for (let i = 0; i < 9; i++) {
                let copy = clone();
                if (copy[i] === 0) {
                    copy[i] = this.computer;
                    if (this.check(copy, this.computer)) {
                        this.mark(i);
                        return;
                    }
                }
            }

            // Can the opponent win next move?
            for (let i = 0; i < 9; i++) {
                let copy = clone();
                if (copy[i] === 0) {
                    copy[i] = this.player;
                    if (this.check(copy, this.player)) {
                        this.mark(i);
                        return;
                    }
                }
            }

            // Can we take a corner?
            let place = ranMove([0, 2, 6, 8]);
            if (!(place === null)) {
                this.mark(place);
                return;
            }

            // Can we take the center?
            if (!this.board[4]) {
                this.mark(4);
                return;
            }

            // Take an edge.
            place = ranMove([1, 3, 5, 7]);
            if (place) {
                this.mark(place);
            }
        }
    }
});