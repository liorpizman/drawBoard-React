import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { DrawArea } from './components/DrawArea';
import Footer from './Footer';

/**
 * App component used to display the Layout
 */
export default class App extends Component {
    displayName = App.name

    render() {
        return (
            <Layout>
                <Route exact path='/' component={Home} />
                <Route path='/drawArea' component={DrawArea} />
                <Footer />
            </Layout>
        );
    }
}
