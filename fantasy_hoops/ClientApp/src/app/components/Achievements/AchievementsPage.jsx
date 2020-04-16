import React from 'react';

import './Achievements.css';
import {isAuth} from "../../utils/auth";
import {Helmet} from "react-helmet";
import {Intro} from "./utils";
import {Canonicals} from "../../utils/helpers";
import Achievements from "./Achievements";

function AchievementsPage() {
    const isLoggedIn = isAuth();
    return (
        <>
            <Helmet>
                <title>Achievements | Fantasy Hoops</title>
                <meta property="title" content="Leaderboards | Fantasy Hoops"/>
                <meta property="og:title" content="Leaderboards | Fantasy Hoops"/>
                <meta name="description" content={isLoggedIn ? Intro.SUBTITLE_AUTH : Intro.SUBTITLE}/>
                <meta property="og:description" content={isLoggedIn ? Intro.SUBTITLE_AUTH : Intro.SUBTITLE}/>
                <meta name="robots" content="index,follow"/>
                <link rel="canonical" href={Canonicals.LEADERBOARDS}/>
            </Helmet>
            <article className="PageIntro">
                <h1 className="PageTitle">{Intro.TITLE}</h1>
                <h5 className="PageSubtitle">{isLoggedIn ? Intro.SUBTITLE_AUTH : Intro.SUBTITLE}</h5>
            </article>
            <Achievements user={isLoggedIn} readOnly={!isLoggedIn}/>
        </>
    );
}

export default AchievementsPage;