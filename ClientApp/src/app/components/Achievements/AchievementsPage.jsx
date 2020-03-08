import React from 'react';

import './Achievements.css';
import {isAuth} from "../../utils/auth";
import {Helmet} from "react-helmet";
import {Intro} from "./utils";
import {Canonicals} from "../../utils/helpers";
import Achievements from "./Achievements";

function AchievementsPage() {
    const loggedIn = isAuth();
    return (
        <>
            <Helmet>
                <title>Achievements | Fantasy Hoops</title>
                <meta property="title" content="Leaderboards | Fantasy Hoops"/>
                <meta property="og:title" content="Leaderboards | Fantasy Hoops"/>
                <meta name="description" content={loggedIn ? Intro.SUBTITLE_AUTH : Intro.SUBTITLE}/>
                <meta property="og:description" content={loggedIn ? Intro.SUBTITLE_AUTH : Intro.SUBTITLE}/>
                <meta name="robots" content="index,follow"/>
                <link rel="canonical" href={Canonicals.LEADERBOARDS}/>
            </Helmet>
            <article className="Leaderboards__Intro">
                <h1 className="Leaderboards__Title">{Intro.TITLE}</h1>
                <h5 className="Leaderboards__Subtitle">{loggedIn ? Intro.SUBTITLE_AUTH : Intro.SUBTITLE}</h5>
            </article>
            <Achievements user={loggedIn} readOnly={!loggedIn}/>
        </>
    );
}

export default AchievementsPage;