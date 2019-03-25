import React, { Component } from 'react';
import _ from 'lodash';
import shortid from 'shortid';
import InfiniteScroll from 'react-infinite-scroll-component';
import { NewsCard } from './NewsCard';
import { getNews } from '../../utils/networkFunctions';

export class NewsFeed extends Component {
  _isMounted = false;

  constructor(props) {
    super(props);
    this.state = {
      news: '',
      hasMore: true,
      newsLoader: false
    };
    this.fetchData = this.fetchData.bind(this);
  }

  async componentDidMount() {
    this._isMounted = true;

    this.setState({
      newsLoader: true
    });
    await getNews()
      .then((res) => {
        if (this._isMounted) {
          this.setState({
            news: res.data,
            newsLoader: false
          });
        }
      });
  }

  componentWillUnmount() {
    this._isMounted = false;
  }

  async fetchData() {
    const { news } = this.state;
    if (this._isMounted) {
      this.setState({
        newsLoader: true
      });
    }

    await getNews({ start: news.length })
      .then((res) => {
        if (this._isMounted) {
          this.setState({
            news: news.concat(res.data),
            hasMore: res.data.length === 6,
            newsLoader: false
          });
        }
      });
  }

  render() {
    const { news, hasMore, newsLoader } = this.state;
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
            next={this.fetchData}
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

export default NewsFeed;
