# Elevator-Sim
This is a Monte Carlo simulation developed to accompany a discrete optimization problem used for enrichment activities in Mathematics summer camps held by Queen’s University.

**Technologies Used:** Unity, C#, WebGL

## The problem
You have been asked to program the elevator by the owner of an apartment building. The building has six floors. Floor 0 is used for entering and exiting the building, and floors 1 through 5 have the same number of residents. The building has one elevator. With it, residents may either leave the building or enter and return to their floor. No resident may take the elevator between floors, and so every call to the elevator is either taking a resident from floor 0 up to their floor or vice versa. Hence, we observe that half of all calls are from floor 0, and go to another floor each with probability $\frac{1}{5}$. The other half comes from the resident floors with equal probability $\frac{1}{5}$, and all go to floor 0.

In this problem, we divide time into equal intervals such that in each interval, there will be at most one event. Either a resident leaving or a resident entering. 

Your task is to program what the elevator should do after it has delivered a resident. It could stay where it is or park on another floor to wait for the next call, perhaps in a more favourable position, subject to costs or constraints. In this problem, we consider two types of costs:
* The energy cost of moving the elevator. We assume that this cost is proportional to the number of floors the elevator travels, regardless of whether a passenger is on board.
* The opportunity cost incurred by the resident in having to wait for the elevator after it has been called. This cost is proportional to the number of floors the elevator travels from its last parking position to the floor the resident has called from.

Let $m$ represent the total number of floors the elevator travels, including the move to a parking position. Also, let $n$ represent the number of floors the elevator travels to pick up a resident after a call has been made. Finally, let $w$ represent the relative weighting of the two costs, where $0 \le w \le 10$. Then, the total cost ($C$) of a single call is calculated as $$C = m + wn$$.

So, your job is to minimize the __expected__ cost of a single call to the elevator.

## About the simulator
In this simulator, you can alter the parking algorithm and relative weighting, and then test it. By randomly simulating a large number of calls, the simulator gives an accurate approximation of the expected cost per call of your parking algorithm. So, whether you are prioritizing energy efficiency or resident satisfaction, run the simulation now to visualize how different parking strategies impact your expected costs.

