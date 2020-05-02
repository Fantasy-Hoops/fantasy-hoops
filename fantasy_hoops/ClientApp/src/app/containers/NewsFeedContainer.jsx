import React, {Component, useEffect, useState} from 'react';
import PropTypes from 'prop-types';
import {connect} from 'react-redux';
import {bindActionCreators} from 'redux';
import _ from 'lodash';
import shortid from 'shortid';
import InfiniteScroll from 'react-infinite-scroll-component';
import NewsCard from '../components/News/NewsCard';
import * as actionCreators from '../actions/news';
import {Container} from "@material-ui/core";

import './NewsFeedContainer.css';
import Tabs from "@material-ui/core/Tabs";
import Tab from "@material-ui/core/Tab";
import {useStyles} from "./NewsFeedContainerStyle";
import Typography from "@material-ui/core/Typography";
import Box from "@material-ui/core/Box";
import moment from "moment";
import {Helmet} from "react-helmet";
import {Canonicals} from "../utils/helpers";

const googleAd = (
    <>
        <script async src="https://pagead2.googlesyndication.com/pagead/js/adsbygoogle.js"></script>
        <ins className="adsbygoogle"
             style={{display: "block"}}
             data-ad-format="fluid"
             data-ad-layout-key="-fb+5w+4e-db+86"
             data-ad-client="ca-pub-6391166063453559"
             data-ad-slot="6919426443"></ins>
        <script>
            (adsbygoogle = window.adsbygoogle || []).push({});
        </script>
    </>
);

const Intro = {
    TITLE: "NEWS",
    SUBTITLE: "Get to know the latest daily articles about NBA games - short previews and recaps available, including " +
        "players' and coaches' thoughts, game breakdowns and summaries. Follow the news regularly to find out " +
        "projected NBA players' status updates, game plan changes and potential rotation for the upcoming games.",
    COPYRIGHT: `${moment().year()} by STATS/Field Level Media.`
};

const mapStateToProps = state => ({
    news: state.newsContainerReducer.news,
    newsLoader: state.newsContainerReducer.newsLoader,
    hasMoreNews: state.newsContainerReducer.hasMoreNews,
    previews: state.newsContainerReducer.previews,
    previewsLoader: state.newsContainerReducer.previewsLoader,
    hasMorePreviews: state.newsContainerReducer.hasMorePreviews,
    recaps: state.newsContainerReducer.recaps,
    recapsLoader: state.newsContainerReducer.recapsLoader,
    hasMoreRecaps: state.newsContainerReducer.hasMoreRecaps
});

const mapDispatchToProps = dispatch => bindActionCreators(actionCreators, dispatch);

function TabPanel(props) {
    const {children, value, index, ...other} = props;

    return (
        <Typography
            component="div"
            role="tabpanel"
            hidden={value !== index}
            id={`news-tabpanel-${index}`}
            aria-labelledby={`simple-tab-${index}`}
            {...other}
        >
            {value === index && <Box className="NewsFeed__Box">{children}</Box>}
        </Typography>
    );
}

function NewsFeedContainer(props) {
    const {
        news, loadNews, loadMoreNews, hasMoreNews, previews, loadPreviews, loadMorePreviews, hasMorePreviews,
        recaps, loadRecaps, loadMoreRecaps, hasMoreRecaps
    } = props;
    const classes = useStyles();
    const [value, setValue] = React.useState(0);

    useEffect(() => {
        loadPreviews();
    }, []);

    const previewsCards = _.map(previews,
        (newsObj, index) => (
            <NewsCard
                key={shortid()}
                index={index}
                news={newsObj}
            />
        ));

    const recapsCards = _.map(recaps,
        (newsObj, index) => (
            <NewsCard
                key={shortid()}
                index={index}
                news={newsObj}
            />
        ));


    const handleChange = (event, newValue) => {
        if (newValue === 0 && previews.length === 0) {
            loadPreviews();
        }
        if (newValue === 1 && recaps.length === 0) {
            loadRecaps();
        }
        setValue(newValue);
    };

    return (
        <>
            <Helmet>
                <title>News | Fantasy Hoops</title>
                <meta property="title" content="News | Fantasy Hoops"/>
                <meta property="og:title" content="News | Fantasy Hoops"/>
                <meta name="description" content={Intro.SUBTITLE}/>
                <meta property="og:description" content={Intro.SUBTITLE}/>
                <meta name="robots" content="index,follow"/>
                <link rel="canonical" href={Canonicals.NEWS}/>
            </Helmet>
            <article className="PageIntro">
                <h1 className="PageTitle">{Intro.TITLE}</h1>
                <h5 className="PageSubtitle">{Intro.SUBTITLE}</h5>
                <h5 className="PageSubtitle">&copy; {Intro.COPYRIGHT}</h5>
            </article>
            <Tabs
                id="NewsFeed__tabs"
                className={classes.tabs}
                value={value}
                indicatorColor="primary"
                textColor="primary"
                onChange={handleChange}
                aria-label="disabled tabs example"
            >
                <Tab label="Previews"/>
                <Tab label="Recaps"/>
            </Tabs>
            <TabPanel value={value} index={0}>
                {googleAd}
                <InfiniteScroll
                    dataLength={previews.length}
                    next={() => loadMorePreviews(previews.length)}
                    hasMore={hasMorePreviews}
                    loader={<div className="Loader"/>}
                >
                    {previewsCards}
                </InfiniteScroll>
            </TabPanel>
            <TabPanel value={value} index={1}>
                {googleAd}
                <InfiniteScroll
                    dataLength={recaps.length}
                    next={() => loadMoreRecaps(recaps.length)}
                    hasMore={hasMoreRecaps}
                    loader={<div className="Loader"/>}
                >
                    {recapsCards}
                </InfiniteScroll>
            </TabPanel>
        </>
    );
}

NewsFeedContainer.propTypes = {
    loadNews: PropTypes.func.isRequired,
    loadMoreNews: PropTypes.func.isRequired,
    news: PropTypes.arrayOf(
        PropTypes.shape({
            id: PropTypes.number.isRequired
        })
    ).isRequired,
    hasMoreNews: PropTypes.bool.isRequired,
    loadPreviews: PropTypes.func.isRequired,
    loadMorePreviews: PropTypes.func.isRequired,
    previews: PropTypes.arrayOf(
        PropTypes.shape({
            id: PropTypes.number.isRequired
        })
    ).isRequired,
    hasMorePreviews: PropTypes.bool.isRequired,
    loadRecaps: PropTypes.func.isRequired,
    loadMoreRecaps: PropTypes.func.isRequired,
    recaps: PropTypes.arrayOf(
        PropTypes.shape({
            id: PropTypes.number.isRequired
        })
    ).isRequired,
    hasMoreRecaps: PropTypes.bool.isRequired
};

export default connect(mapStateToProps, mapDispatchToProps)(NewsFeedContainer);
