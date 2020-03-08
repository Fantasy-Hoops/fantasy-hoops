import React from 'react';

import './Achievements.css';
import {isAuth} from "../../utils/auth";
import {Helmet} from "react-helmet";
import {Intro} from "./utils";
import {Canonicals} from "../../utils/helpers";
import Achievements from "./Achievements";

function AchievementsPage() {
    const user = isAuth();

    return (
        <>
            <Helmet>
                <title>Achievements | Fantasy Hoops</title>
                <meta property="title" content="Leaderboards | Fantasy Hoops"/>
                <meta property="og:title" content="Leaderboards | Fantasy Hoops"/>
                <meta name="description" content={user ? Intro.SUBTITLE_AUTH : Intro.SUBTITLE}/>
                <meta property="og:description" content={user ? Intro.SUBTITLE_AUTH : Intro.SUBTITLE}/>
                <meta name="robots" content="index,follow"/>
                <link rel="canonical" href={Canonicals.LEADERBOARDS}/>
            </Helmet>
            <article className="Leaderboards__Intro">
                <h1 className="Leaderboards__Title">{Intro.TITLE}</h1>
                <h5 className="Leaderboards__Subtitle">{user ? Intro.SUBTITLE_AUTH : Intro.SUBTITLE}</h5>
            </article>
            <Achievements user={user} readOnly={!user}/>
        </>
    );
}

export default AchievementsPage;