const functions = require("firebase-functions");
const admin = require("firebase-admin");
// let UserCount = 0;
admin.initializeApp();
exports.getDailyLeaderboard = functions
    .region("us-central1")
    .runWith({
        timeoutSeconds: 60,
        memory: "128MB",
    })
    .https
    .onCall(async (data, context) => {
        //const uid = context.auth?.uid; you can get the userId with this;
        const NumResults = data.NumResults;

        if (NumResults === undefined)
            return { users: [] };

        const total = parseInt(NumResults);
        return await admin
            .database()
            .ref("DailyLeaderboard/Users")
            .orderByChild("userScore")
            .limitToLast(total)
            .once("value")
            .then((snapshot) => {
                console.log(snapshot.val());
                if (snapshot.val() !== null) {
                    const results = [];
                    let index = snapshot.numChildren();
                    snapshot.forEach((user) => {
                        results.unshift({
                            sessionCounter: user.child("sessionCounter").val(),
                            assetID: user.child("assetID").val(),
                            userName: user.child("userName").val(),
                            userScore: user.child("userScore").val(),
                            userRank: index,
                        });
                        index--;
                    });
                    return { users: results };
                } else {
                    return { users: [] };
                }
            })
            .catch((error) => {
                console.log(error);
                return { users: [] };
            });
    });
exports.getLeaderboard = functions
    .region("us-central1")
    .runWith({
        timeoutSeconds: 60,
        memory: "128MB",
    })
    .https
    .onCall(async (data, context) => {
        //const uid = context.auth?.uid; you can get the userId with this;
        const NumResults = data.NumResults;
        if (NumResults === undefined)
            return { users: [] };

        const total = parseInt(NumResults);
        return await admin
            .database()
            .ref("Leaderboard/Users")
            .orderByChild("userScore")
            .limitToLast(total)
            .once("value")
            .then((snapshot) => {
                console.log(snapshot.val());
                if (snapshot.val() !== null) {
                    const results = [];
                    let index = snapshot.numChildren();
                    snapshot.forEach((user) => {
                        results.unshift({
                            sessionCounter: user.child("sessionCounter").val(),
                            assetID: user.child("assetID").val(),
                            userName: user.child("userName").val(),
                            userScore: user.child("userScore").val(),
                            userRank: index,
                        });
                        index--;
                    });
                    return { users: results };
                } else {
                    return { users: [] };
                }
            })
            .catch((error) => {
                console.log(error);
                return { users: [] };
            });
    });