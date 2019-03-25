import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import _ from 'lodash';
import shortid from 'shortid';
import InfiniteScroll from 'react-infinite-scroll-component';
import { NewsCard } from '../components/News/NewsCard';
import * as actionCreators from '../actions/news';

const mapStateToProps = state => ({
  news: state.newsContainerReducer.news,
  newsLoader: state.newsContainerReducer.newsLoader,
  hasMore: state.newsContainerReducer.hasMore
});

const mapDispatchToProps = dispatch => bindActionCreators(actionCreators, dispatch);


export class NewsFeedContainer extends Component {
  async componentDidMount() {
    const { loadNews } = this.props;
    await loadNews();
  }

  render() {
    const {
      news, hasMore, loadMoreNews
    } = this.props;
    const newsCards = _.map(news,
      newsObj => (
        <NewsCard
          key={shortid()}
          news={newsObj}
        />
      ));
    return (
      <div className="container bg-light pt-5">
        <div className="center col">
          <InfiniteScroll
            dataLength={news.length}
            next={loadMoreNews}
            hasMore={hasMore}
            loader={<div className="Loader" />}
          >
            {newsCards}
          </InfiniteScroll>
        </div>
      </div>
    );
  }
}

NewsFeedContainer.propTypes = {
  loadNews: PropTypes.func.isRequired,
  loadMoreNews: PropTypes.func.isRequired,
  news: PropTypes.arrayOf(
    PropTypes.shape({
      id: PropTypes.number.isRequired
    })
  ).isRequired,
  hasMore: PropTypes.bool.isRequired
};

export default connect(mapStateToProps, mapDispatchToProps)(NewsFeedContainer);
