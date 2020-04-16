import React, {useState} from 'react';
import Scroll from 'react-scroll';
import shortid from 'shortid';
import _ from 'lodash';

import './NewsCard.css';
import Img from 'react-image';
import defaultLogo from '../../../content/images/defaultLogo.png';
import moment from "moment";

const PARAGRAPHS_AMOUNT = 3;

function NewsCard(props) {
    const {index, news} = props;
    const [expanded, setExpanded] = useState(false);

    function toggleExpanded(e) {
        const div = e.target.parentElement;
        const position = div.getBoundingClientRect().top - 120;
        const duration = (position * -1) / 15;
        if (position < 0) {
            Scroll.animateScroll.scrollMore(position, {
                duration,
                smooth: true
            });
        }
        setExpanded(!expanded);
    }

    const paragraphs = _.map(news.paragraphs,
        paragraph => <p key={shortid()} className="NewsCard__Content">{paragraph}</p>);

    return (
        <>
            <article className="NewsCard">
                <h2 id={`NewsCard__${index}`} className="NewsCard__Title">
                    {news.title}
                </h2>
                <div className="NewsCard__Date"><span>{moment(news.date).format("MMM D, YYYY")}</span></div>
                <div className="NewsCard__Logos">
                    <Img
                        className="NewsCard__TeamLogo"
                        alt={news.hTeam}
                        src={[
                            `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/logos/${news.vTeam}.svg`,
                            defaultLogo
                        ]}
                        loader={<img height="50px" src={require('../../../content/images/imageLoader.gif')}
                                     alt="Loader"/>}
                        decode={false}
                    />
                    <span className="NewsCard__Label--vs">vs.</span>
                    <Img
                        className="NewsCard__TeamLogo"
                        alt={news.vTeam}
                        src={[
                            `${process.env.REACT_APP_IMAGES_SERVER_NAME}/content/images/logos/${news.hTeam}.svg`,
                            defaultLogo
                        ]}
                        loader={<img height="50px" src={require('../../../content/images/imageLoader.gif')}
                                     alt="Loader"/>}
                        decode={false}
                    />
                </div>
                <input
                    onChange={toggleExpanded}
                    checked={expanded}
                    type="checkbox"
                    className="read-more-state"
                    id={news.id}
                />
                <article className="read-more-wrap" style={{textAlign: 'justify'}}>
                    {paragraphs.slice(0, 1)}
                    {index === 0
                        ? <>
                            <script async src="https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js"/>
                            <ins className="adsbygoogle"
                                 style={{display: 'block', textAlign: 'center'}}
                                 data-ad-layout="in-article"
                                 data-ad-format="fluid"
                                 data-ad-client="ca-pub-6391166063453559"
                                 data-ad-slot="5855491321">
                            </ins>
                            <script>
                                (adsbygoogle = window.adsbygoogle || []).push({});
                            </script>
                        </>
                        : ''}
                    {paragraphs.slice(1, PARAGRAPHS_AMOUNT)}
                    <span className="read-more-target">
                      {!expanded ? '' : paragraphs.slice(PARAGRAPHS_AMOUNT)}
                    </span>
                </article>
                <label htmlFor={news.id} className="read-more-trigger"/>
            </article>
        </>
    )
}

export default NewsCard;
