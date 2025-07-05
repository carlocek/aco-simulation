# Ant Colony Optimization (ACO) Simulation
This Unity project visually simulates the Ant Colony Optimization (ACO) algorithm for solving the **Traveling Salesman Problem (TSP)**. Ants explore a graph of connected nodes and gradually find the shortest path through **pheromone-based reinforcement**, mimicking real-world ant behavior.

## Overview
The ACO algorithm is a nature-inspired optimization technique that uses the following principles:
- **Ants explore paths** between nodes (cities) randomly.
- Each ant completes a full tour visiting all nodes once.
- After each tour, ants **deposit pheromones** on the paths they traveled.
- Paths with **more pheromones** become more attractive in future iterations.
- Over time, **shorter paths accumulate more pheromones**, guiding the colony toward the optimal route.

This project visualizes:
- Ants moving step-by-step through the graph.
- Animated edges as ants travel.
- Pheromone intensities displayed as semi-transparent lines.
- Real-time UI controls to adjust speed and simulation parameters.

## How to Run
You can run this application by downloading the latest pre-built executable:
1. Go to the **Releases** tab on this GitHub repository.
2. Download the `.zip` file.
3. Extract the archive.
4. Run the executable (e.g., `aco-simulation.exe`).

> No installation or dependencies required — everything is included!
