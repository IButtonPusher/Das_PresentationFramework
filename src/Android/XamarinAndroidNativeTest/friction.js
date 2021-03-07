function auswahl2() {

    if (document.rechner2.weg.value != "") {
        check = check + 1;
    }
    if (document.rechner2.zeit.value != "") {
        check = check + 2;
    }
    if (document.rechner2.geschwindigkeit.value != "") {
        check = check + 4;
    }
    if (document.rechner2.geschwindigkeit_v0.value != "") {
        check = check + 16;
    }
    if (document.rechner2.beschleunigung.value != "") {
        check = check + 8;
    }
    if (document.rechner2.reaktionszeit.value != "") {
        check = check + 32;
    }
    if (document.rechner2.anhalteweg.value != "") {
        check = check + 64;
    }
}

function berechnen2() {

    brakingDistance = parseFloat(document.rechner2.weg.value.replace(",", "."));
    brakingTime = parseFloat(document.rechner2.zeit.value.replace(",", "."));
    finalVelocity = parseFloat(document.rechner2.geschwindigkeit.value.replace(",", "."));
    initialVelocity = parseFloat(document.rechner2.geschwindigkeit_v0.value.replace(",", "."));
    acceleration = parseFloat(document.rechner2.beschleunigung.value.replace(",", "."));
    reactionTime = parseFloat(document.rechner2.reaktionszeit.value.replace(",", "."));
    stoppingDistance = parseFloat(document.rechner2.anhalteweg.value.replace(",", "."));

    farbef = 0;

    document.getElementById("meldung").style.color = "green";

    if (brakingTime < 0) {
        resetfarbe1();
        document.getElementById("meldung").style.color = "red";
        document.getElementById("meldung").innerHTML = "Time has to be a positive number!";
        farbef = 1;
    }

    if (finalVelocity < 0) {
        resetfarbe1();
        document.getElementById("meldung").style.color = "red";
        document.getElementById("meldung").innerHTML = "Final velocity has to be a positive number!";
        farbef = 1;
    }

    if (stoppingDistance < 0) {
        resetfarbe1();
        document.getElementById("meldung").style.color = "red";
        document.getElementById("meldung").innerHTML = "Overall stopping distance has to be a positive number!";
        farbef = 1;
    }

    if (reactionTime < 0) {
        resetfarbe1();
        document.getElementById("meldung").style.color = "red";
        document.getElementById("meldung").innerHTML = "Reaction time has to be a positive number!";
        farbef = 1;
    }

    if (brakingDistance < 0) {
        resetfarbe1();
        document.getElementById("meldung").style.color = "red";
        document.getElementById("meldung").innerHTML = "Braking distance has to be a positive number!";
        farbef = 1;
    }

    if (initialVelocity < 0) {
        resetfarbe1();
        document.getElementById("meldung").style.color = "red";
        document.getElementById("meldung").innerHTML = "Starting velocitiy has to be a positive number!";
        farbef = 1;
    }

    if (isNaN(document.rechner2.weg.value.replace(",", ".")) || isNaN(document.rechner2.zeit.value.replace(",", ".")) || isNaN(document.rechner2.geschwindigkeit.value.replace(",", ".")) || isNaN(document.rechner2.acceleration.value.replace(",", ".")) || isNaN(document.rechner2.geschwindigkeit_v0.value.replace(",", ".")) || isNaN(document.rechner2.reactionTime.value.replace(",", ".")) || isNaN(document.rechner2.anhalteweg.value.replace(",", "."))) {
        resetfarbe1();
        document.getElementById("meldung").style.color = "red";
        document.getElementById("meldung").innerHTML = "One of your inputs is no number!";
    } else {



        if (farbef == 0) {

            if (check == 51) {
                pruef = 1;
                finalVelocity = (2 * brakingDistance * 3.6 - initialVelocity * brakingTime) / brakingTime;
                acceleration = (finalVelocity * finalVelocity - initialVelocity * initialVelocity) / (2 * brakingDistance * 3.6 * 3.6);
                stoppingDistance = brakingDistance + initialVelocity * reactionTime / 3.6;

                acceleration = acceleration.toFixed(3);
                finalVelocity = finalVelocity.toFixed(3);
                stoppingDistance = stoppingDistance.toFixed(3);

                resetfarbe1();
                document.rechner2.geschwindigkeit.value = finalVelocity;
                document.rechner2.geschwindigkeit.style.background = '#67e580';
                document.rechner2.acceleration.value = acceleration;
                document.rechner2.acceleration.style.background = '#67e580';
                document.rechner2.anhalteweg.value = stoppingDistance;
                document.rechner2.anhalteweg.style.background = '#67e580';

                if (finalVelocity < 0 || stoppingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative overall stopping distance and/or negative final velocity - please change values!";
                }

            }


            if (check == 114) {
                pruef = 1;

                brakingDistance = stoppingDistance - initialVelocity * reactionTime / 3.6;
                finalVelocity = (2 * brakingDistance * 3.6 - initialVelocity * brakingTime) / brakingTime;
                acceleration = (finalVelocity * finalVelocity - initialVelocity * initialVelocity) / (2 * brakingDistance * 3.6 * 3.6);

                acceleration = acceleration.toFixed(3);
                finalVelocity = finalVelocity.toFixed(3);
                brakingDistance = brakingDistance.toFixed(3);

                resetfarbe1();
                document.rechner2.geschwindigkeit.value = finalVelocity;
                document.rechner2.geschwindigkeit.style.background = '#67e580';
                document.rechner2.acceleration.value = acceleration;
                document.rechner2.acceleration.style.background = '#67e580';
                document.rechner2.weg.value = brakingDistance;
                document.rechner2.weg.style.background = '#67e580';

                if (finalVelocity < 0 || brakingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative braking distance and/or negative final velocity - please change values!";
                }

            }


            if (check == 53) {
                pruef = 1;
                acceleration = (finalVelocity * finalVelocity - initialVelocity * initialVelocity) / (2 * brakingDistance * 3.6 * 3.6);
                brakingTime = (finalVelocity - initialVelocity) / (3.6 * acceleration);

                stoppingDistance = brakingDistance + initialVelocity * reactionTime / 3.6;
                stoppingDistance = stoppingDistance.toFixed(3);
                acceleration = acceleration.toFixed(3);
                brakingTime = brakingTime.toFixed(3);

                resetfarbe1();
                document.rechner2.zeit.value = brakingTime;
                document.rechner2.zeit.style.background = '#67e580';
                document.rechner2.acceleration.value = acceleration;
                document.rechner2.acceleration.style.background = '#67e580';
                document.rechner2.anhalteweg.value = stoppingDistance;
                document.rechner2.anhalteweg.style.background = '#67e580';

                if (brakingTime < 0 || stoppingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative overall stopping distance and/or negative breaking time - please change values!";
                }
            }


            if (check == 116) {
                pruef = 1;

                brakingDistance = stoppingDistance - initialVelocity * reactionTime / 3.6;
                acceleration = (finalVelocity * finalVelocity - initialVelocity * initialVelocity) / (2 * brakingDistance * 3.6 * 3.6);
                brakingTime = (finalVelocity - initialVelocity) / (3.6 * acceleration);

                brakingDistance = brakingDistance.toFixed(3);
                acceleration = acceleration.toFixed(3);
                brakingTime = brakingTime.toFixed(3);

                resetfarbe1();
                document.rechner2.zeit.value = brakingTime;
                document.rechner2.zeit.style.background = '#67e580';
                document.rechner2.acceleration.value = acceleration;
                document.rechner2.acceleration.style.background = '#67e580';
                document.rechner2.weg.value = brakingDistance;
                document.rechner2.weg.style.background = '#67e580';

                if (brakingTime < 0 || brakingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative braking distance and/or negative braking time - please change values!";
                }
            }

            if (check == 54) {
                pruef = 1;
                acceleration = (finalVelocity - initialVelocity) / (brakingTime * 3.6);
                brakingDistance = (finalVelocity * brakingTime) / (2 * 3.6) + (brakingTime * initialVelocity) / (3.6 * 2);

                stoppingDistance = brakingDistance + initialVelocity * reactionTime / 3.6;
                stoppingDistance = stoppingDistance.toFixed(3);
                brakingDistance = brakingDistance.toFixed(3);
                acceleration = acceleration.toFixed(3);

                resetfarbe1();
                document.rechner2.weg.value = brakingDistance;
                document.rechner2.weg.style.background = '#67e580';
                document.rechner2.acceleration.value = acceleration;
                document.rechner2.acceleration.style.background = '#67e580';
                document.rechner2.anhalteweg.value = stoppingDistance;
                document.rechner2.anhalteweg.style.background = '#67e580';

                if (brakingDistance < 0 || stoppingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative braking distance and/or negative overall stopping distance - please change values!";
                }
            }

            if (check == 57) {
                pruef = 1;

                finalVelocity = Math.sqrt(initialVelocity * initialVelocity / (3.6 * 3.6) + 2 * brakingDistance * acceleration) * 3.6;
                brakingTime = (finalVelocity - initialVelocity) / (acceleration * 3.6);

                stoppingDistance = brakingDistance + initialVelocity * reactionTime / 3.6;
                stoppingDistance = stoppingDistance.toFixed(3);
                finalVelocity = finalVelocity.toFixed(3);
                brakingTime = brakingTime.toFixed(3);

                resetfarbe1();

                if (brakingTime < 0 || finalVelocity < 0 || stoppingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative result - please change values!";
                }

                if (isNaN(finalVelocity)) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Calculation of final velocity isn't possible - please change values!";
                } else {
                    document.rechner2.zeit.value = brakingTime;
                    document.rechner2.zeit.style.background = '#67e580';
                    document.rechner2.geschwindigkeit.value = finalVelocity;
                    document.rechner2.geschwindigkeit.style.background = '#67e580';
                    document.rechner2.anhalteweg.value = stoppingDistance;
                    document.rechner2.anhalteweg.style.background = '#67e580';
                }
            }


            if (check == 120) {
                pruef = 1;

                brakingDistance = stoppingDistance - initialVelocity * reactionTime / 3.6;
                finalVelocity = Math.sqrt(initialVelocity * initialVelocity / (3.6 * 3.6) + 2 * brakingDistance * acceleration) * 3.6;
                brakingTime = (finalVelocity - initialVelocity) / (acceleration * 3.6);

                brakingDistance = brakingDistance.toFixed(3);
                finalVelocity = finalVelocity.toFixed(3);
                brakingTime = brakingTime.toFixed(3);

                resetfarbe1();

                if (brakingTime < 0 || finalVelocity < 0 || brakingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative result - please change values!";
                }

                if (isNaN(finalVelocity)) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Calculation of final velocity isn't possible - please change values!";
                } else {
                    document.rechner2.zeit.value = brakingTime;
                    document.rechner2.zeit.style.background = '#67e580';
                    document.rechner2.geschwindigkeit.value = finalVelocity;
                    document.rechner2.geschwindigkeit.style.background = '#67e580';
                    document.rechner2.weg.value = brakingDistance;
                    document.rechner2.weg.style.background = '#67e580';
                }
            }


            ////////////////////////////

            // accel/starting velocity/braking time

            if (check == 58) {
                pruef = 1;
                finalVelocity = acceleration * brakingTime * 3.6 + initialVelocity;
                brakingDistance = (acceleration * brakingTime * brakingTime) / 2 + initialVelocity * brakingTime / 3.6;

                stoppingDistance = brakingDistance + initialVelocity * reactionTime / 3.6;
                stoppingDistance = stoppingDistance.toFixed(3);
                brakingDistance = brakingDistance.toFixed(3);
                finalVelocity = finalVelocity.toFixed(3);

                resetfarbe1();
                document.rechner2.weg.value = brakingDistance;
                document.rechner2.weg.style.background = '#67e580';
                document.rechner2.geschwindigkeit.value = finalVelocity;
                document.rechner2.geschwindigkeit.style.background = '#67e580';
                document.rechner2.anhalteweg.value = stoppingDistance;
                document.rechner2.anhalteweg.style.background = '#67e580';

                if (brakingDistance < 0 || finalVelocity < 0 || stoppingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative result - please change values!";
                }
            }

            /////////////////////////////

            // accel/starting velocity/final velocity

            if (check == 60) {
                pruef = 1;
                brakingTime = (finalVelocity - initialVelocity) / (acceleration * 3.6);
                brakingDistance = (finalVelocity * finalVelocity - initialVelocity * initialVelocity) / (2 * acceleration * 3.6 * 3.6);
                stoppingDistance = brakingDistance + initialVelocity * reactionTime / 3.6;
                stoppingDistance = stoppingDistance.toFixed(3);

                brakingTime = brakingTime.toFixed(3);
                brakingDistance = brakingDistance.toFixed(3);

                resetfarbe1();

                if (brakingDistance < 0 || brakingTime < 0) {
                    document.rechner2.weg.value = "";
                    document.rechner2.anhalteweg.value = "";
                    document.rechner2.zeit.value = "";


                    if (finalVelocity > initialVelocity) {
                        finalVelocity = finalVelocity.toFixed(1);
                        initialVelocity = initialVelocity.toFixed(1);

                        document.getElementById("meldung").style.color = "red";
                        document.getElementById("meldung").innerHTML = "If the final velocity should be greater than the starting velocity, it have to be accelerated. Therefore, in the field acceleration/deceleration must be a positive number!";
                    }

                    if (finalVelocity < initialVelocity) {
                        finalVelocity = finalVelocity.toFixed(1);
                        initialVelocity = initialVelocity.toFixed(1);

                        document.getElementById("meldung").style.color = "red";
                        document.getElementById("meldung").innerHTML = "If the starting velocity should be greater than the final velocity, it have to be braked. Therefore, in the field acceleration/deceleration must be a negative number!";
                    }
                } else {
                    document.rechner2.weg.value = brakingDistance;
                    document.rechner2.weg.style.background = '#67e580';
                    document.rechner2.zeit.value = brakingTime;
                    document.rechner2.zeit.style.background = '#67e580';
                    document.rechner2.anhalteweg.value = stoppingDistance;
                    document.rechner2.anhalteweg.style.background = '#67e580';
                }
            }

            /////////////////////////////


            if (check == 45) {
                pruef = 1;
                initialVelocity = Math.sqrt(finalVelocity * finalVelocity / (3.6 * 3.6) - (2 * brakingDistance * acceleration)) * 3.6;
                brakingTime = (finalVelocity - initialVelocity) / (acceleration * 3.6);
                stoppingDistance = brakingDistance + initialVelocity * reactionTime / 3.6;
                stoppingDistance = stoppingDistance.toFixed(3);

                brakingTime = brakingTime.toFixed(3);
                initialVelocity = initialVelocity.toFixed(3);

                resetfarbe1();

                if (brakingTime < 0 || stoppingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative overall stopping distance and/or negative braking time - please change values!";
                }

                if (isNaN(brakingTime) || isNaN(initialVelocity)) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Calculation of starting velocity isn't possible - please change values!";
                } else {
                    document.rechner2.geschwindigkeit_v0.value = initialVelocity;
                    document.rechner2.geschwindigkeit_v0.style.background = '#67e580';
                    document.rechner2.zeit.value = brakingTime;
                    document.rechner2.zeit.style.background = '#67e580';
                    document.rechner2.anhalteweg.value = stoppingDistance;
                    document.rechner2.anhalteweg.style.background = '#67e580';
                }
            }


            if (check == 108) {
                pruef = 1;


                if (acceleration < 0) {
                    brakingDistance = (-acceleration * reactionTime * reactionTime + stoppingDistance) - Math.sqrt((-acceleration * reactionTime * reactionTime + stoppingDistance) * (-acceleration * reactionTime * reactionTime + stoppingDistance) - stoppingDistance * stoppingDistance + finalVelocity * finalVelocity * reactionTime * reactionTime / (3.6 * 3.6));
                } else {
                    brakingDistance = (-acceleration * reactionTime * reactionTime + stoppingDistance) + Math.sqrt((-acceleration * reactionTime * reactionTime + stoppingDistance) * (-acceleration * reactionTime * reactionTime + stoppingDistance) - stoppingDistance * stoppingDistance + finalVelocity * finalVelocity * reactionTime * reactionTime / (3.6 * 3.6));
                }

                initialVelocity = Math.sqrt(finalVelocity * finalVelocity / (3.6 * 3.6) - (2 * brakingDistance * acceleration)) * 3.6;
                brakingTime = (finalVelocity - initialVelocity) / (acceleration * 3.6);
                brakingDistance = brakingDistance.toFixed(3);

                brakingTime = brakingTime.toFixed(3);
                initialVelocity = initialVelocity.toFixed(3);

                resetfarbe1();


                if (initialVelocity < 0 || brakingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative starting velocity and/or negative braking distance - please change values!";
                }

                if (isNaN(brakingDistance) || isNaN(initialVelocity)) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Calculation of starting velocity isn't possible - please change values!";
                } else {
                    document.rechner2.geschwindigkeit_v0.value = initialVelocity;
                    document.rechner2.geschwindigkeit_v0.style.background = '#67e580';
                    document.rechner2.zeit.value = brakingTime;
                    document.rechner2.zeit.style.background = '#67e580';
                    document.rechner2.weg.value = brakingDistance;
                    document.rechner2.weg.style.background = '#67e580';
                }

            }


            if (check == 46) {
                pruef = 1;
                initialVelocity = (finalVelocity / 3.6 - acceleration * brakingTime) * 3.6;
                brakingDistance = (finalVelocity * brakingTime) / 3.6 - (acceleration * brakingTime * brakingTime / 2);
                stoppingDistance = brakingDistance + initialVelocity * reactionTime / 3.6;
                stoppingDistance = stoppingDistance.toFixed(3);

                brakingDistance = brakingDistance.toFixed(3);
                initialVelocity = initialVelocity.toFixed(3);

                resetfarbe1();
                document.rechner2.geschwindigkeit_v0.value = initialVelocity;
                document.rechner2.geschwindigkeit_v0.style.background = '#67e580';
                document.rechner2.weg.value = brakingDistance;
                document.rechner2.weg.style.background = '#67e580';
                document.rechner2.anhalteweg.value = stoppingDistance;
                document.rechner2.anhalteweg.style.background = '#67e580';

                if (initialVelocity < 0 || brakingDistance < 0 || stoppingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative starting velocity and/or negative braking/overall stopping distance - please change values!";
                }
            }

            if (check == 43) {
                pruef = 1;
                finalVelocity = (brakingDistance / brakingTime + (acceleration * brakingTime) / 2) * 3.6;
                initialVelocity = finalVelocity - acceleration * brakingTime * 3.6;
                stoppingDistance = brakingDistance + initialVelocity * reactionTime / 3.6;
                stoppingDistance = stoppingDistance.toFixed(3);

                finalVelocity = finalVelocity.toFixed(3);
                initialVelocity = initialVelocity.toFixed(3);

                resetfarbe1();
                document.rechner2.geschwindigkeit_v0.value = initialVelocity;
                document.rechner2.geschwindigkeit_v0.style.background = '#67e580';
                document.rechner2.geschwindigkeit.value = finalVelocity;
                document.rechner2.geschwindigkeit.style.background = '#67e580';
                document.rechner2.anhalteweg.value = stoppingDistance;
                document.rechner2.anhalteweg.style.background = '#67e580';

                if (initialVelocity < 0 || finalVelocity < 0 || stoppingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative starting/final velocity and/or negative overall stopping distance - please change values!";
                }
            }


            if (check == 106) {
                pruef = 1;

                brakingDistance = (stoppingDistance + acceleration * brakingTime * reactionTime / 2) / (1 + reactionTime / brakingTime);
                finalVelocity = (brakingDistance / brakingTime + (acceleration * brakingTime) / 2) * 3.6;
                initialVelocity = finalVelocity - acceleration * brakingTime * 3.6;

                brakingDistance = brakingDistance.toFixed(3);
                finalVelocity = finalVelocity.toFixed(3);
                initialVelocity = initialVelocity.toFixed(3);

                resetfarbe1();
                document.rechner2.geschwindigkeit_v0.value = initialVelocity;
                document.rechner2.geschwindigkeit_v0.style.background = '#67e580';
                document.rechner2.geschwindigkeit.value = finalVelocity;
                document.rechner2.geschwindigkeit.style.background = '#67e580';
                document.rechner2.weg.value = brakingDistance;
                document.rechner2.weg.style.background = '#67e580';

                if (initialVelocity < 0 || finalVelocity < 0 || brakingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative starting/final velocity and/or negative braking distance - please change values!";
                }
            }


            if (check == 39) {
                pruef = 1;
                acceleration = (2 * (finalVelocity * brakingTime / 3.6 - brakingDistance)) / (brakingTime * brakingTime);
                initialVelocity = finalVelocity - acceleration * brakingTime * 3.6;

                stoppingDistance = brakingDistance + initialVelocity * reactionTime / 3.6;
                stoppingDistance = stoppingDistance.toFixed(3);

                acceleration = acceleration.toFixed(3);
                initialVelocity = initialVelocity.toFixed(3);

                resetfarbe1();
                document.rechner2.geschwindigkeit_v0.value = initialVelocity;
                document.rechner2.geschwindigkeit_v0.style.background = '#67e580';
                document.rechner2.acceleration.value = acceleration;
                document.rechner2.acceleration.style.background = '#67e580';
                document.rechner2.anhalteweg.value = stoppingDistance;
                document.rechner2.anhalteweg.style.background = '#67e580';

                if (initialVelocity < 0 || stoppingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative starting velocity and/or negative overall stopping distance - please change values!";
                }
            }


            if (check == 102) {
                pruef = 1;

                brakingDistance = (stoppingDistance + finalVelocity * reactionTime / 3.6) / (1 + 2 * reactionTime / brakingTime);
                acceleration = (2 * (finalVelocity * brakingTime / 3.6 - brakingDistance)) / (brakingTime * brakingTime);
                initialVelocity = finalVelocity - acceleration * brakingTime * 3.6;

                brakingDistance = brakingDistance.toFixed(3);
                acceleration = acceleration.toFixed(3);
                initialVelocity = initialVelocity.toFixed(3);

                resetfarbe1();
                document.rechner2.geschwindigkeit_v0.value = initialVelocity;
                document.rechner2.geschwindigkeit_v0.style.background = '#67e580';
                document.rechner2.acceleration.value = acceleration;
                document.rechner2.acceleration.style.background = '#67e580';
                document.rechner2.weg.value = brakingDistance;
                document.rechner2.weg.style.background = '#67e580';

                if (initialVelocity < 0 || brakingDistance < 0) {
                    document.getElementById("meldung").style.color = "red";
                    document.getElementById("meldung").innerHTML = "Negative starting velocity and/or negative braking distance - please change values!";
                }
            }

            acceleration = Math.abs(parseFloat(document.rechner2.acceleration.value.replace(",", ".")));

            if (abfragewert == 1 || abfragewert == 2 || abfragewert == 3 || abfragewert == 4) {

                if (acceleration >= 6.5) {
                    document.getElementById("abfragewerte").selectedIndex = 0;
                }
                if (acceleration < 6.5 && acceleration >= 3.5) {
                    document.getElementById("abfragewerte").selectedIndex = 1;
                }
                if (acceleration < 3.5 && acceleration >= 1.5) {
                    document.getElementById("abfragewerte").selectedIndex = 2;
                }
                if (acceleration < 1.5) {
                    document.getElementById("abfragewerte").selectedIndex = 3;
                }
            } else {
                if (acceleration >= 1.75) {
                    document.getElementById("abfragewerte").selectedIndex = 4;
                }
                if (acceleration < 1.75 && acceleration >= 1.25) {
                    document.getElementById("abfragewerte").selectedIndex = 5;
                }
                if (acceleration < 1.25) {
                    document.getElementById("abfragewerte").selectedIndex = 6;
                }
            }


            if (document.rechner2.weg.value != "" && document.rechner2.anhalteweg.value != "" && document.getElementById("meldung").style.color != "red") {
                var reaktionswegv = Math.abs(stoppingDistance - brakingDistance);
                reaktionswegv = reaktionswegv.toFixed(3);

                document.getElementById("meldung").style.color = "green";
                document.getElementById("meldung").innerHTML = "Thinking distance is " + reaktionswegv + " m.";
            }

            if (pruef == 0) {
                resetfarbe1();
                document.getElementById("meldung").style.color = "red";
                if (document.rechner2.weg.value != "" && document.rechner2.anhalteweg.value != "") {
                    document.getElementById("meldung").innerHTML = "Only the braking distance or the stopping distance may be filled out!";
                } else {
                    document.getElementById("meldung").innerHTML = "There has to be a number in 3 out of the 6 fields and in the field 'reaction time'!";
                }
            }
        }
    }
}